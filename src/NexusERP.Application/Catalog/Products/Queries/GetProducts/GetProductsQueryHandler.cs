using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Catalog.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    private readonly INexusDbContext _context;

    public GetProductsQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Tax)
            .Include(p => p.Variants)
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Category!.Name,
                p.Brand != null ? p.Brand.Name : null,
                p.Tax != null ? p.Tax.Name : null,
                p.CabysCode,
                p.MeasurementUnit,
                p.IsActive,
                p.Variants.Select(v => new ProductVariantDto(
                    v.Id, v.Sku, v.Barcode, v.Size, v.Color, v.Cost, v.Price, v.IsActive
                )).ToList()
            ))
            .ToListAsync(cancellationToken);
    }
}
