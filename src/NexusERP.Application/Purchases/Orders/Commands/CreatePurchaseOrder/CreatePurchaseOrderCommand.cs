using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Purchases;
using NexusERP.Domain.Entities.Finance;

namespace NexusERP.Application.Purchases.Orders.Commands.CreatePurchaseOrder;

public record CreatePurchaseOrderCommand(
    Guid SupplierId,
    Guid BranchId,
    DateTime ExpectedDeliveryDate,
    string Notes,
    List<PurchaseOrderDetailCommand> Details
) : IRequest<Guid>;

public record PurchaseOrderDetailCommand(
    Guid ProductVariantId,
    string ProductName,
    string Sku,
    decimal QuantityOrdered,
    decimal UnitCost
);

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreatePurchaseOrderCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        // Obtener proveedor
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId, cancellationToken);
            
        if (supplier == null)
            throw new Exception("El proveedor seleccionado no existe.");

        // Generar OrderNumber (PO-000001)
        var lastOrder = await _context.PurchaseOrders
            .OrderByDescending(o => o.OrderDate)
            .FirstOrDefaultAsync(cancellationToken);
            
        int nextNumber = 1;
        if (lastOrder != null && lastOrder.OrderNumber.StartsWith("PO-"))
        {
            if (int.TryParse(lastOrder.OrderNumber.Replace("PO-", ""), out int lastNum))
            {
                nextNumber = lastNum + 1;
            }
        }
        string orderNumber = $"PO-{nextNumber:D6}";

        var order = new PurchaseOrder
        {
            SupplierId = request.SupplierId,
            SupplierName = supplier.Name,
            BranchId = request.BranchId,
            OrderNumber = orderNumber,
            OrderDate = DateTime.UtcNow,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            Status = "Sent", // Podría ser Draft, lo marcamos Sent para simplificar
            Notes = request.Notes
        };

        decimal totalAmount = 0;

        foreach (var detail in request.Details)
        {
            var subtotal = detail.QuantityOrdered * detail.UnitCost;
            totalAmount += subtotal;

            order.Details.Add(new PurchaseOrderDetail
            {
                ProductVariantId = detail.ProductVariantId,
                ProductName = detail.ProductName,
                Sku = detail.Sku,
                QuantityOrdered = detail.QuantityOrdered,
                QuantityReceived = 0, // Aún no se ha recibido
                UnitCost = detail.UnitCost,
                Subtotal = subtotal
            });
        }

        order.TotalAmount = totalAmount;

        // Crear Cuenta por Pagar (AccountsPayable) pendiente (opcional, algunos lo crean hasta recibir, 
        // pero creémosla aquí en status Pending para ir rastreando la deuda proyectada).
        var accountsPayable = new AccountsPayable
        {
            SupplierId = supplier.Id,
            PurchaseOrderId = order.Id, // Referencia futura cuando EF asigne ID
            InvoiceNumber = "PENDING-" + orderNumber, // El proveedor luego nos da la factura real
            OriginalAmount = totalAmount,
            BalanceDue = totalAmount,
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(30), // Por defecto 30 días crédito
            Status = "Pending"
        };
        
        _context.PurchaseOrders.Add(order);
        _context.AccountsPayables.Add(accountsPayable);
        
        await _context.SaveChangesAsync(cancellationToken);

        // Actualizar la referencia del PurchaseOrderId en la cuenta por pagar ya guardada si fuese necesario, 
        // pero EF Core lo hace en memoria antes del SaveChanges si hay navegación de objetos. 
        // Aquí lo forzamos manual post-guardado si la relación no es explícita:
        accountsPayable.PurchaseOrderId = order.Id;
        await _context.SaveChangesAsync(cancellationToken);

        return order.Id;
    }
}
