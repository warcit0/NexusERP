using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;
using NexusERP.Infrastructure.Persistence.Interceptors;
using NexusERP.Domain.Entities.Catalog;
using NexusERP.Domain.Entities.Sales;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Entities.Purchases;
using NexusERP.Domain.Entities.Finance;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NexusERP.Infrastructure.Identity;

namespace NexusERP.Infrastructure.Persistence;

public class NexusDbContext : IdentityDbContext<ApplicationUser>, INexusDbContext
{
    private readonly IMediator _mediator;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly ICurrentTenantService _currentTenantService;

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();
    public DbSet<CashRegisterSession> CashRegisterSessions => Set<CashRegisterSession>();
    
    public DbSet<Tax> Taxes => Set<Tax>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleDetail> SaleDetails => Set<SaleDetail>();
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<NexusERP.Domain.Entities.Inventory.InventoryTransaction> InventoryTransactions => Set<NexusERP.Domain.Entities.Inventory.InventoryTransaction>();
    public DbSet<NexusERP.Domain.Entities.Inventory.InventoryBalance> InventoryBalances => Set<NexusERP.Domain.Entities.Inventory.InventoryBalance>();
    
    // Compras
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
    
    // Finanzas
    public DbSet<AccountsReceivable> AccountsReceivables => Set<AccountsReceivable>();
    public DbSet<CustomerPayment> CustomerPayments => Set<CustomerPayment>();
    public DbSet<AccountsPayable> AccountsPayables => Set<AccountsPayable>();
    public DbSet<SupplierPayment> SupplierPayments => Set<SupplierPayment>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

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

        // Configuración de schemas y filtros
        builder.Entity<Branch>().ToTable("Branches", "tenant");
        builder.Entity<CashRegister>().ToTable("CashRegisters", "tenant");
        builder.Entity<CashRegisterSession>().ToTable("CashRegisterSessions", "tenant");
        
        builder.Entity<Tax>().ToTable("Taxes", "tenant");
        builder.Entity<Category>().ToTable("Categories", "tenant");
        builder.Entity<Brand>().ToTable("Brands", "tenant");
        builder.Entity<Product>().ToTable("Products", "tenant");
        builder.Entity<ProductVariant>().ToTable("ProductVariants", "tenant");
        
        builder.Entity<Customer>().ToTable("Customers", "tenant");
        builder.Entity<Sale>().ToTable("Sales", "tenant");
        builder.Entity<SaleDetail>().ToTable("SaleDetails", "tenant");
        builder.Entity<Payment>().ToTable("Payments", "tenant");
        
        builder.Entity<Supplier>().ToTable("Suppliers", "tenant");
        builder.Entity<PurchaseOrder>().ToTable("PurchaseOrders", "tenant");
        builder.Entity<PurchaseOrderDetail>().ToTable("PurchaseOrderDetails", "tenant");
        
        builder.Entity<AccountsReceivable>().ToTable("AccountsReceivables", "tenant");
        builder.Entity<CustomerPayment>().ToTable("CustomerPayments", "tenant");
        builder.Entity<AccountsPayable>().ToTable("AccountsPayables", "tenant");
        builder.Entity<SupplierPayment>().ToTable("SupplierPayments", "tenant");

        builder.Entity<NexusERP.Domain.Entities.Inventory.InventoryTransaction>().ToTable("InventoryTransactions", "tenant");
        builder.Entity<NexusERP.Domain.Entities.Inventory.InventoryBalance>().ToTable("InventoryBalances", "tenant");
        
        // Filtro global para aislamiento de Tenants
        builder.Entity<Branch>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<CashRegister>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<CashRegisterSession>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        
        builder.Entity<Tax>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<Category>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<Brand>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<Product>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<ProductVariant>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        
        builder.Entity<Customer>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<Sale>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<SaleDetail>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<Payment>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        
        builder.Entity<Supplier>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<PurchaseOrder>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<PurchaseOrderDetail>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        
        builder.Entity<AccountsReceivable>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<CustomerPayment>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<AccountsPayable>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<SupplierPayment>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        
        builder.Entity<NexusERP.Domain.Entities.Inventory.InventoryTransaction>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        builder.Entity<NexusERP.Domain.Entities.Inventory.InventoryBalance>().HasQueryFilter(e => e.TenantId == _currentTenantService.TenantId);
        
        // Relaciones complejas
        builder.Entity<ProductVariant>()
            .HasOne(pv => pv.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(pv => pv.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Sale>()
            .HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Sale>()
            .HasOne(s => s.Session)
            .WithMany()
            .HasForeignKey(s => s.CashRegisterSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InventoryTransaction>()
            .HasOne(it => it.Branch)
            .WithMany()
            .HasForeignKey(it => it.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.Entity<InventoryBalance>()
            .HasOne(ib => ib.Branch)
            .WithMany()
            .HasForeignKey(ib => ib.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ApplyConfigurationsFromAssembly(typeof(NexusDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Auto-asignar TenantId a todas las entidades TenantEntity que se estén agregando
        var tenantId = _currentTenantService.TenantId;
        if (tenantId.HasValue && tenantId.Value != Guid.Empty)
        {
            foreach (var entry in ChangeTracker.Entries<TenantEntity>()
                         .Where(e => e.State == EntityState.Added))
            {
                if (entry.Entity.TenantId == Guid.Empty)
                {
                    entry.Entity.TenantId = tenantId.Value;
                }
            }
        }

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
