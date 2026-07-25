using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Purchases.Orders.Queries.GetPurchaseOrders;

public record GetPurchaseOrdersQuery : IRequest<List<PurchaseOrderDto>>;

public record PurchaseOrderDto(
    Guid Id,
    string OrderNumber,
    Guid SupplierId,
    string SupplierName,
    Guid BranchId,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount
);

public class GetPurchaseOrdersQueryHandler : IRequestHandler<GetPurchaseOrdersQuery, List<PurchaseOrderDto>>
{
    private readonly INexusDbContext _context;

    public GetPurchaseOrdersQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<PurchaseOrderDto>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        return await _context.PurchaseOrders
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new PurchaseOrderDto(
                o.Id,
                o.OrderNumber,
                o.SupplierId,
                o.SupplierName,
                o.BranchId,
                o.OrderDate,
                o.Status,
                o.TotalAmount
            ))
            .ToListAsync(cancellationToken);
    }
}
