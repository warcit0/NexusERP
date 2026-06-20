using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities;

namespace NexusERP.Application.Common.Interfaces;

public interface INexusDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<License> Licenses { get; }
    DbSet<Branch> Branches { get; }
    DbSet<CashRegister> CashRegisters { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
