using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities;

using NexusERP.Domain.Entities.Catalog;
using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Application.Common.Interfaces;

public interface INexusDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }
    DbSet<License> Licenses { get; }
    
    // Configuración y Ventas
    DbSet<Branch> Branches { get; }
    DbSet<CashRegister> CashRegisters { get; }
    DbSet<CashRegisterSession> CashRegisterSessions { get; }
    
    // Catálogo
    DbSet<Tax> Taxes { get; }
    DbSet<Category> Categories { get; }
    DbSet<Brand> Brands { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductVariant> ProductVariants { get; }

    DbSet<NexusERP.Domain.Entities.Sales.Customer> Customers { get; }
    DbSet<NexusERP.Domain.Entities.Sales.Sale> Sales { get; }
    DbSet<NexusERP.Domain.Entities.Sales.SaleDetail> SaleDetails { get; }
    DbSet<NexusERP.Domain.Entities.Sales.Payment> Payments { get; }

    DbSet<NexusERP.Domain.Entities.Inventory.InventoryTransaction> InventoryTransactions { get; }
    DbSet<NexusERP.Domain.Entities.Inventory.InventoryBalance> InventoryBalances { get; }
    
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
