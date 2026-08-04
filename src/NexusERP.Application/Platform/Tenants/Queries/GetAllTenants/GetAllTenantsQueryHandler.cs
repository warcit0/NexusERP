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
            .ToListAsync(cancellationToken);

        // Get active licenses for plan info (for tenants that don't have CurrentSubscriptionPlanId set)
        var tenantIds = tenants.Select(t => t.Id).ToList();
        var activeLicenses = await _context.Licenses
            .Include(l => l.SubscriptionPlan)
            .Where(l => tenantIds.Contains(l.TenantId) && l.IsActive)
            .ToListAsync(cancellationToken);

        var licenseByTenant = activeLicenses
            .GroupBy(l => l.TenantId)
            .ToDictionary(g => g.Key, g => g.FirstOrDefault());

        return tenants.Select(t =>
        {
            string planName = "Sin plan";
            if (t.CurrentSubscriptionPlan != null)
                planName = t.CurrentSubscriptionPlan.Name;
            else if (licenseByTenant.TryGetValue(t.Id, out var lic) && lic?.SubscriptionPlan != null)
                planName = lic.SubscriptionPlan.Name;

            return new TenantDto(
                t.Id,
                t.Name,
                t.Subdomain,
                t.IsActive ? "Active" : "Inactive",
                t.IsActive,
                planName,
                DateTime.UtcNow,
                0
            );
        }).ToList();
    }
}
