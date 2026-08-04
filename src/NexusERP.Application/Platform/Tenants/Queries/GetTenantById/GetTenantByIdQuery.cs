using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Platform.Tenants.Queries.GetTenantById;

public record GetTenantByIdQuery(Guid Id) : IRequest<TenantDetailDto?>;

public record TenantDetailDto(
    Guid Id,
    string Name,
    string CommercialName,
    string TaxId,
    string Subdomain,
    string Email,
    string Phone,
    bool IsActive
);

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantDetailDto?>
{
    private readonly INexusDbContext _context;

    public GetTenantByIdQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<TenantDetailDto?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var t = await _context.Tenants
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (t == null) return null;

        return new TenantDetailDto(
            t.Id,
            t.Name,
            t.CommercialName,
            t.TaxId,
            t.Subdomain,
            t.Email,
            t.Phone,
            t.IsActive
        );
    }
}
