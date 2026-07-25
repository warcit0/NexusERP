using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Application.Purchases.Orders.Commands.ReceivePurchaseOrder;

public record ReceivePurchaseOrderCommand(
    Guid PurchaseOrderId,
    string SupplierInvoiceNumber, // Factura final del proveedor
    List<ReceivePurchaseOrderDetailCommand> ReceivedDetails
) : IRequest<bool>;

public record ReceivePurchaseOrderDetailCommand(
    Guid PurchaseOrderDetailId,
    decimal QuantityReceived
);

public class ReceivePurchaseOrderCommandHandler : IRequestHandler<ReceivePurchaseOrderCommand, bool>
{
    private readonly INexusDbContext _context;

    public ReceivePurchaseOrderCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ReceivePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == request.PurchaseOrderId, cancellationToken);

        if (order == null)
            throw new Exception("La orden de compra no existe.");

        if (order.Status == "Received")
            throw new Exception("La orden de compra ya fue recibida completamente.");

        bool allReceived = true;

        foreach (var receivedDetail in request.ReceivedDetails)
        {
            var detail = order.Details.FirstOrDefault(d => d.Id == receivedDetail.PurchaseOrderDetailId);
            if (detail != null && receivedDetail.QuantityReceived > 0)
            {
                detail.QuantityReceived += receivedDetail.QuantityReceived;

                if (detail.QuantityReceived < detail.QuantityOrdered)
                {
                    allReceived = false;
                }

                // 1. Ingresar transacción de inventario
                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    ProductVariantId = detail.ProductVariantId,
                    BranchId = order.BranchId,
                    TransactionType = "Entrada",
                    Quantity = receivedDetail.QuantityReceived,
                    Reference = $"Compra {order.OrderNumber} - Fac {request.SupplierInvoiceNumber}",
                    TransactionDate = DateTime.UtcNow
                });

                // 2. Actualizar saldo de inventario
                var balance = await _context.InventoryBalances
                    .FirstOrDefaultAsync(b => b.ProductVariantId == detail.ProductVariantId && b.BranchId == order.BranchId, cancellationToken);

                if (balance == null)
                {
                    _context.InventoryBalances.Add(new InventoryBalance
                    {
                        ProductVariantId = detail.ProductVariantId,
                        BranchId = order.BranchId,
                        CurrentStock = receivedDetail.QuantityReceived,
                        LastUpdated = DateTime.UtcNow
                    });
                }
                else
                {
                    balance.CurrentStock += receivedDetail.QuantityReceived;
                    balance.LastUpdated = DateTime.UtcNow;
                }
            }
        }

        order.Status = allReceived ? "Received" : "PartiallyReceived";
        order.ReceivedDate = DateTime.UtcNow;

        // 3. Actualizar la cuenta por pagar con el número de factura final
        var accountsPayable = await _context.AccountsPayables
            .FirstOrDefaultAsync(ap => ap.PurchaseOrderId == order.Id, cancellationToken);

        if (accountsPayable != null && !string.IsNullOrEmpty(request.SupplierInvoiceNumber))
        {
            accountsPayable.InvoiceNumber = request.SupplierInvoiceNumber;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
