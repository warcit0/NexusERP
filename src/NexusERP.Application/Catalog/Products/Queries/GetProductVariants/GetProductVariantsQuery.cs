using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Catalog.Products.Queries.GetProductVariants;

public record GetProductVariantsQuery : IRequest<List<FlatProductVariantDto>>;

public record FlatProductVariantDto(
    Guid Id,
    string ProductName,
    string Sku,
    decimal Cost,
    decimal Price
);

public class GetProductVariantsQueryHandler : IRequestHandler<GetProductVariantsQuery, List<FlatProductVariantDto>>
{
    private readonly INexusDbContext _context;

    public GetProductVariantsQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<FlatProductVariantDto>> Handle(GetProductVariantsQuery request, CancellationToken cancellationToken)
    {
        var query = await _context.ProductVariants
            .Include(v => v.Product)
            .Where(v => v.IsActive && v.Product != null && v.Product.IsActive)
            .OrderBy(v => v.Product!.Name)
            .Select(v => new
            {
                v.Id,
                ProductName = v.Product!.Name,
                v.Sku,
                v.Cost,
                v.Price
            })
            .ToListAsync(cancellationToken);

        return query.Select(x => new FlatProductVariantDto(
            x.Id,
            x.ProductName,
            x.Sku,
            x.Cost,
            x.Price
        )).ToList();
    }
}
