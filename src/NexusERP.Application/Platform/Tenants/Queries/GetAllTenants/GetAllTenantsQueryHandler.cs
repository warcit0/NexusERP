using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Platform.Tenants.Queries.GetAllTenants;

public class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, List<TenantDto>>
{
    private readonly INexusDbContext _context;

    public GetAllTenantsQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        var tenants = await _context.Tenants
            .Include(t => t.CurrentSubscriptionPlan)
            .OrderBy(t => t.Name)
            .Select(t => new TenantDto(
                t.Id,
                t.Name,
                t.Subdomain,
                t.IsActive ? "Activo" : "Inactivo",
                t.CurrentSubscriptionPlan != null ? t.CurrentSubscriptionPlan.Name : "Sin plan",
                DateTime.UtcNow, // BaseEntity no expone CreatedAt, se agrega en Fase 2
                0
            ))
            .ToListAsync(cancellationToken);

        return tenants;
    }
}
