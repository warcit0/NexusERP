using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Sales;
using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Application.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateSaleCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // 1. Validar que la sesión de caja exista y esté abierta
        var session = await _context.CashRegisterSessions
            .FirstOrDefaultAsync(s => s.Id == request.CashRegisterSessionId && s.ClosedAt == null, cancellationToken);
            
        if (session == null)
            throw new Exception("La sesión de caja seleccionada no existe o ya está cerrada.");

        // 2. Generar un número de recibo consecutivo simple
        var lastSale = await _context.Sales
            .OrderByDescending(s => s.Date)
            .FirstOrDefaultAsync(cancellationToken);
            
        int nextNumber = 1;
        if (lastSale != null && lastSale.ReceiptNumber.StartsWith("TKT-"))
        {
            if (int.TryParse(lastSale.ReceiptNumber.Replace("TKT-", ""), out int lastNum))
            {
                nextNumber = lastNum + 1;
            }
        }
        string receiptNumber = $"TKT-{nextNumber:D6}";

        // 3. Calcular totales y crear Entidad Venta
        var sale = new Sale
        {
            CashRegisterSessionId = request.CashRegisterSessionId,
            BranchId = request.BranchId,
            CustomerId = request.CustomerId,
            ReceiptNumber = receiptNumber,
            Date = DateTime.UtcNow,
            Status = "Completada"
        };

        decimal subtotal = 0;
        decimal taxTotal = 0;

        foreach (var detail in request.Details)
        {
            var lineSubtotal = detail.Quantity * detail.UnitPrice;
            var lineTax = lineSubtotal * (detail.TaxPercentage / 100m);
            var lineTotal = lineSubtotal + lineTax;

            subtotal += lineSubtotal;
            taxTotal += lineTax;

            sale.Details.Add(new SaleDetail
            {
                ProductVariantId = detail.ProductVariantId,
                ProductName = detail.ProductName,
                Sku = detail.Sku,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                TaxPercentage = detail.TaxPercentage,
                Subtotal = lineSubtotal,
                Total = lineTotal
            });

            // 4. Actualizar Kardex (Transacción de Salida)
            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                ProductVariantId = detail.ProductVariantId,
                BranchId = request.BranchId,
                TransactionType = "Salida",
                Quantity = -detail.Quantity, // Negativo porque es venta
                Reference = $"Venta {receiptNumber}",
                TransactionDate = DateTime.UtcNow
            });

            // 5. Actualizar Saldo de Inventario
            var balance = await _context.InventoryBalances
                .FirstOrDefaultAsync(b => b.ProductVariantId == detail.ProductVariantId && b.BranchId == request.BranchId, cancellationToken);

            if (balance == null)
            {
                balance = new InventoryBalance
                {
                    ProductVariantId = detail.ProductVariantId,
                    BranchId = request.BranchId,
                    CurrentStock = -detail.Quantity, // Permitimos stock negativo
                    LastUpdated = DateTime.UtcNow
                };
                _context.InventoryBalances.Add(balance);
            }
            else
            {
                balance.CurrentStock -= detail.Quantity;
                balance.LastUpdated = DateTime.UtcNow;
            }
        }

        sale.Subtotal = subtotal;
        sale.TaxTotal = taxTotal;
        sale.Total = subtotal + taxTotal;

        // 6. Registrar Pagos
        foreach (var payment in request.Payments)
        {
            sale.Payments.Add(new Payment
            {
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Reference = payment.Reference,
                PaymentDate = DateTime.UtcNow
            });
        }

        // Validación extra: Verificar que el pago cubra el total
        var totalPaid = request.Payments.Sum(p => p.Amount);
        if (totalPaid < sale.Total)
        {
            throw new Exception("El monto pagado es menor al total de la venta.");
        }

        _context.Sales.Add(sale);
        
        // 7. Guardar todo en una única transacción
        await _context.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }
}
