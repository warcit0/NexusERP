using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Inventory.Queries.GetInventoryBalances;

public class GetInventoryBalancesQueryHandler : IRequestHandler<GetInventoryBalancesQuery, List<InventoryBalanceDto>>
{
    private readonly INexusDbContext _context;

    public GetInventoryBalancesQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryBalanceDto>> Handle(GetInventoryBalancesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.InventoryBalances
            .Include(ib => ib.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Include(ib => ib.Branch)
            .AsNoTracking();

        if (request.BranchId.HasValue)
        {
            query = query.Where(ib => ib.BranchId == request.BranchId.Value);
        }

        return await query
            .Select(ib => new InventoryBalanceDto
            {
                ProductVariantId = ib.ProductVariantId,
                ProductName = ib.ProductVariant.Product.Name,
                Sku = ib.ProductVariant.Sku,
                Barcode = ib.ProductVariant.Barcode,
                Size = ib.ProductVariant.Size,
                Color = ib.ProductVariant.Color,
                BranchId = ib.BranchId,
                BranchName = ib.Branch.Name,
                CurrentStock = ib.CurrentStock,
                LastUpdated = ib.LastUpdated
            })
            .ToListAsync(cancellationToken);
    }
}
