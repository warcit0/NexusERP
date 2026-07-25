using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Purchases.Suppliers.Queries.GetSuppliers;

public record GetSuppliersQuery : IRequest<List<SupplierDto>>;

public record SupplierDto(Guid Id, string Name, string TaxId, string Email, string Phone, string Address, bool IsActive);

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, List<SupplierDto>>
{
    private readonly INexusDbContext _context;

    public GetSuppliersQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplierDto>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        return await _context.Suppliers
            .OrderBy(s => s.Name)
            .Select(s => new SupplierDto(s.Id, s.Name, s.TaxId, s.Email, s.Phone, s.Address, s.IsActive))
            .ToListAsync(cancellationToken);
    }
}
