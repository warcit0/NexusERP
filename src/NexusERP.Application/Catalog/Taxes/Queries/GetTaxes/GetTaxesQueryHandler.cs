using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Catalog.Taxes.Queries.GetTaxes;

public class GetTaxesQueryHandler : IRequestHandler<GetTaxesQuery, List<TaxDto>>
{
    private readonly INexusDbContext _context;

    public GetTaxesQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaxDto>> Handle(GetTaxesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Taxes
            .OrderBy(t => t.Name)
            .Select(t => new TaxDto(t.Id, t.Name, t.Percentage, t.IsActive))
            .ToListAsync(cancellationToken);
    }
}
