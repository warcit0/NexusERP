using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;

namespace NexusERP.Application.Platform.Licenses.Commands.AssignLicense;

public record AssignLicenseCommand(Guid TenantId, Guid PlanId, int DurationMonths) : IRequest<bool>;

public class AssignLicenseCommandHandler : IRequestHandler<AssignLicenseCommand, bool>
{
    private readonly INexusDbContext _context;

    public AssignLicenseCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(AssignLicenseCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.PlanId, cancellationToken);
        
        if (tenant == null || plan == null) return false;

        // Inactivar licencias anteriores del inquilino
        var activeLicenses = await _context.Licenses
            .Where(l => l.TenantId == request.TenantId && l.IsActive)
            .ToListAsync(cancellationToken);
            
        foreach (var l in activeLicenses)
        {
            l.IsActive = false;
        }
        
        // Crear nueva licencia
        var newLicense = new License
        {
            TenantId = request.TenantId,
            SubscriptionPlanId = request.PlanId,
            LicenseKey = Guid.NewGuid().ToString("N").ToUpper(), // Generate a simple key
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(request.DurationMonths),
            IsActive = true
        };
        
        tenant.CurrentSubscriptionPlanId = request.PlanId;
        tenant.CurrentSubscriptionPlan = plan; // Update the tenant's current plan reference

        _context.Licenses.Add(newLicense);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
