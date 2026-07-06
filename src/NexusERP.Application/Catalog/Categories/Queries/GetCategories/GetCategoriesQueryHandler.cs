using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Catalog.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly INexusDbContext _context;

    public GetCategoriesQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IsActive))
            .ToListAsync(cancellationToken);
    }
}
