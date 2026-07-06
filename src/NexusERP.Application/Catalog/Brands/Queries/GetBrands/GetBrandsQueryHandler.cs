using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Catalog.Brands.Queries.GetBrands;

public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, List<BrandDto>>
{
    private readonly INexusDbContext _context;

    public GetBrandsQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<BrandDto>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Brands
            .OrderBy(b => b.Name)
            .Select(b => new BrandDto(b.Id, b.Name, b.Description, b.IsActive))
            .ToListAsync(cancellationToken);
    }
}
