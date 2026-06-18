using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;
using NexusERP.Infrastructure.Persistence.Interceptors;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NexusERP.Infrastructure.Identity;

namespace NexusERP.Infrastructure.Persistence;

public class NexusDbContext : IdentityDbContext<ApplicationUser>, INexusDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly ICurrentTenantService _currentTenantService;

    public NexusDbContext(
        DbContextOptions<NexusDbContext> options,
        IMediator mediator,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor,
        ICurrentTenantService currentTenantService) 
        : base(options)
    {
        _mediator = mediator;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _currentTenantService = currentTenantService;
    }

    // DbSets se agregarán en las próximas fases

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("platform");

        builder.ApplyConfigurationsFromAssembly(typeof(NexusDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchDomainEvents();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEvents()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _mediator.Publish(domainEvent);
    }
}
