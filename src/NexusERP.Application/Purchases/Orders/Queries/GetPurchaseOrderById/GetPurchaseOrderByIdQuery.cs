using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Purchases.Orders.Queries.GetPurchaseOrderById;

public record GetPurchaseOrderByIdQuery(Guid OrderId) : IRequest<PurchaseOrderDetailDto?>;

public record PurchaseOrderDetailDto(
    Guid Id,
    string OrderNumber,
    string SupplierName,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    List<PurchaseOrderLineDto> Details
);

public record PurchaseOrderLineDto(
    Guid Id,
    Guid ProductVariantId,
    string ProductName,
    string Sku,
    decimal QuantityOrdered,
    decimal QuantityReceived,
    decimal UnitCost,
    decimal Subtotal
);

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, PurchaseOrderDetailDto?>
{
    private readonly INexusDbContext _context;

    public GetPurchaseOrderByIdQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrderDetailDto?> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.PurchaseOrders
            .Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null) return null;

        return new PurchaseOrderDetailDto(
            order.Id,
            order.OrderNumber,
            order.SupplierName,
            order.OrderDate,
            order.Status,
            order.TotalAmount,
            order.Details.Select(d => new PurchaseOrderLineDto(
                d.Id,
                d.ProductVariantId,
                d.ProductName,
                d.Sku,
                d.QuantityOrdered,
                d.QuantityReceived,
                d.UnitCost,
                d.Subtotal
            )).ToList()
        );
    }
}
