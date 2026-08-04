using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Platform.Licenses.Queries.GetLicenses;

public record LicenseDto(Guid Id, Guid TenantId, string TenantName, Guid PlanId, string PlanName, string LicenseKey, DateTime StartDate, DateTime EndDate, bool IsActive);

public record GetLicensesQuery() : IRequest<List<LicenseDto>>;

public class GetLicensesQueryHandler : IRequestHandler<GetLicensesQuery, List<LicenseDto>>
{
    private readonly INexusDbContext _context;

    public GetLicensesQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<LicenseDto>> Handle(GetLicensesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Licenses
            .Include(l => l.Tenant)
            .Include(l => l.SubscriptionPlan)
            .Select(l => new LicenseDto(
                l.Id, 
                l.TenantId, 
                l.Tenant!.Name, 
                l.SubscriptionPlanId, 
                l.SubscriptionPlan!.Name, 
                l.LicenseKey, 
                l.StartDate, 
                l.EndDate, 
                l.IsActive))
            .ToListAsync(cancellationToken);
    }
}
