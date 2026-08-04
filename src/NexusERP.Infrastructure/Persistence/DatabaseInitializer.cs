using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexusERP.Infrastructure.Identity;
using NexusERP.Domain.Entities;
using NexusERP.Domain.Entities.Catalog;
using NexusERP.Domain.Entities.Sales;
using NexusERP.Domain.Entities.Purchases;
using NexusERP.Domain.Entities.Inventory;
using NexusERP.Domain.Entities.Finance;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NexusERP.Infrastructure.Persistence;

public class DatabaseInitializer
{
    private readonly NexusDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        NexusDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error al ejecutar las migraciones de la base de datos.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocurrió un error al poblar la base de datos.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // 1. Seed Roles (idempotente: solo se crean si no existen)
        var roles = new[] { "SuperAdmin", "TenantAdmin", "User" };
        foreach (var role in roles)
        {
            if (await _roleManager.FindByNameAsync(role) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Subscription Plans (idempotente)
        if (!await _context.SubscriptionPlans.AnyAsync())
        {
            _context.SubscriptionPlans.AddRange(
                new SubscriptionPlan { Name = "Basic", MaxUsers = 5, MaxBranches = 1, MonthlyPrice = 29.99m, AnnualPrice = 299.99m },
                new SubscriptionPlan { Name = "Pro", MaxUsers = 20, MaxBranches = 5, MonthlyPrice = 99.99m, AnnualPrice = 999.99m },
                new SubscriptionPlan { Name = "Enterprise", MaxUsers = 100, MaxBranches = 50, MonthlyPrice = 299.99m, AnnualPrice = 2999.99m }
            );
            await _context.SaveChangesAsync();
        }

        // 3. Seed SuperAdmin User (idempotente)
        var superAdminEmail = "admin@nexuserp.com";
        var superAdmin = await _userManager.FindByEmailAsync(superAdminEmail);
        
        if (superAdmin == null)
        {
            superAdmin = new ApplicationUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "Admin",
                IsActive = true
            };

            var result = await _userManager.CreateAsync(superAdmin, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }

        // 4. Seed Multi-Tenant Ecosystem SOLO si no existen tenants
        if (!await _context.Tenants.AnyAsync())
        {
            var tenantA_Id = await SeedNexusTechAsync(superAdmin.Id);
            await SeedFreshMarketAsync();
            await SeedEliteServicesAsync();

            // Asignar el SuperAdmin al primer tenant para que no vea la pantalla en blanco al inicio
            superAdmin.TenantId = tenantA_Id;
            await _userManager.UpdateAsync(superAdmin);
        }
        else
        {
            // Si ya hay tenants, asegurar que el SuperAdmin tenga un TenantId asignado
            if (superAdmin.TenantId == null || superAdmin.TenantId == Guid.Empty)
            {
                var firstTenant = await _context.Tenants.OrderBy(t => t.Name).FirstOrDefaultAsync();
                if (firstTenant != null)
                {
                    superAdmin.TenantId = firstTenant.Id;
                    await _userManager.UpdateAsync(superAdmin);
                }
            }
            _logger.LogInformation("Seed: Tenants ya existen, omitiendo siembra de datos para iniciar más rápido.");
        }
    }

    private async Task ClearDomainDataAsync()
    {
        _context.CustomerPayments.RemoveRange(_context.CustomerPayments);
        _context.SupplierPayments.RemoveRange(_context.SupplierPayments);
        _context.AccountsReceivables.RemoveRange(_context.AccountsReceivables);
        _context.AccountsPayables.RemoveRange(_context.AccountsPayables);
        _context.SaleDetails.RemoveRange(_context.SaleDetails);
        _context.Sales.RemoveRange(_context.Sales);
        _context.PurchaseOrderDetails.RemoveRange(_context.PurchaseOrderDetails);
        _context.PurchaseOrders.RemoveRange(_context.PurchaseOrders);
        _context.InventoryTransactions.RemoveRange(_context.InventoryTransactions);
        _context.InventoryBalances.RemoveRange(_context.InventoryBalances);
        _context.ProductVariants.RemoveRange(_context.ProductVariants);
        _context.Products.RemoveRange(_context.Products);
        _context.Categories.RemoveRange(_context.Categories);
        _context.Brands.RemoveRange(_context.Brands);
        _context.CashRegisterSessions.RemoveRange(_context.CashRegisterSessions);
        _context.CashRegisters.RemoveRange(_context.CashRegisters);
        _context.Branches.RemoveRange(_context.Branches);
        _context.Customers.RemoveRange(_context.Customers);
        _context.Suppliers.RemoveRange(_context.Suppliers);
        _context.Licenses.RemoveRange(_context.Licenses);
        _context.Tenants.RemoveRange(_context.Tenants);
        
        await _context.SaveChangesAsync();
    }

    private async Task<Guid> SeedNexusTechAsync(string adminUserId)
    {
        // Crear Tenant
        var tenant = new Tenant { Name = "NexusTech Electronics", TaxId = "3-101-555555", Email = "info@nexustech.cr", Phone = "2222-1111", IsActive = true };
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        var tId = tenant.Id;

        // Crear Admin
        await CreateTenantAdminAsync("admin@nexustech.cr", tId, "Admin", "NexusTech");

        // Licencia
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == "Pro");
        _context.Licenses.Add(new License { TenantId = tId, SubscriptionPlanId = plan?.Id ?? Guid.Empty, LicenseKey = Guid.NewGuid().ToString("N").ToUpper()[..16], IsActive = true, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1) });
        
        // Sucursales y Cajas
        var b1 = new Branch { TenantId = tId, Name = "Sede Central (San José)", Address = "Sabana Sur", IsActive = true };
        var b2 = new Branch { TenantId = tId, Name = "Sucursal Escazú", Address = "Multiplaza", IsActive = true };
        _context.Branches.AddRange(b1, b2);
        await _context.SaveChangesAsync();

        var cr1 = new CashRegister { TenantId = tId, BranchId = b1.Id, Name = "Caja Principal", IsActive = true, IsOpen = true };
        var cr2 = new CashRegister { TenantId = tId, BranchId = b1.Id, Name = "Caja Servicio Técnico", IsActive = true };
        var cr3 = new CashRegister { TenantId = tId, BranchId = b2.Id, Name = "Caja Escazú 1", IsActive = true };
        _context.CashRegisters.AddRange(cr1, cr2, cr3);

        // Abrir Sesión en cr1
        var session = new CashRegisterSession { TenantId = tId, CashRegister = cr1, OpenedByUserId = adminUserId, OpenedAt = DateTime.UtcNow.AddHours(-4), InitialAmount = 50000 };
        _context.CashRegisterSessions.Add(session);

        // Catálogo (Categorías, Marcas, Productos)
        var catLaptops = new Category { TenantId = tId, Name = "Laptops", IsActive = true };
        var catSmartphones = new Category { TenantId = tId, Name = "Smartphones", IsActive = true };
        var catAcc = new Category { TenantId = tId, Name = "Accesorios", IsActive = true };
        var catRedes = new Category { TenantId = tId, Name = "Redes", IsActive = true };
        _context.Categories.AddRange(catLaptops, catSmartphones, catAcc, catRedes);

        var p1 = new Product { TenantId = tId, Category = catLaptops, Name = "MacBook Pro M3", IsActive = true };
        p1.Variants.Add(new ProductVariant { TenantId = tId, Sku = "MBP-M3-01", Cost = 950000, Price = 1200000, IsActive = true });
        
        var p2 = new Product { TenantId = tId, Category = catLaptops, Name = "Dell XPS 15", IsActive = true };
        p2.Variants.Add(new ProductVariant { TenantId = tId, Sku = "XPS-15-01", Cost = 800000, Price = 1050000, IsActive = true });

        var p3 = new Product { TenantId = tId, Category = catSmartphones, Name = "iPhone 15 Pro", IsActive = true };
        p3.Variants.Add(new ProductVariant { TenantId = tId, Sku = "IP15P-01", Cost = 500000, Price = 750000, IsActive = true });

        var p4 = new Product { TenantId = tId, Category = catSmartphones, Name = "Samsung Galaxy S24", IsActive = true };
        p4.Variants.Add(new ProductVariant { TenantId = tId, Sku = "SGS24-01", Cost = 450000, Price = 680000, IsActive = true });

        var p5 = new Product { TenantId = tId, Category = catAcc, Name = "AirPods Pro 2", IsActive = true };
        p5.Variants.Add(new ProductVariant { TenantId = tId, Sku = "APP2-01", Cost = 100000, Price = 150000, IsActive = true });

        _context.Products.AddRange(p1, p2, p3, p4, p5);

        // Contactos
        var c1 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "TechCorp S.A.", Identification = "3-101-111111", Email = "compras@techcorp.com", IsActive = true, CreditLimit = 2000000 };
        var c2 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "Juan Developer", Identification = "1-1111-1111", Email = "juan@dev.com", IsActive = true };
        var c3 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "María Designer", Identification = "2-2222-2222", Email = "maria@design.com", IsActive = true };
        var c4 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "Carlos Admin", Identification = "1-3333-3333", Email = "carlos@admin.com", IsActive = true };
        _context.Customers.AddRange(c1, c2, c3, c4);

        var s1 = new Supplier { TenantId = tId, Name = "Distribuidora Apple", TaxId = "3-101-222222", Email = "ventas@apple.cr", IsActive = true };
        var s2 = new Supplier { TenantId = tId, Name = "Dell Mayorista", TaxId = "3-101-333333", Email = "ventas@dell.cr", IsActive = true };
        _context.Suppliers.AddRange(s1, s2);

        await _context.SaveChangesAsync(); // Para generar los IDs

        // Ventas
        var sale1 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c1, Date = DateTime.UtcNow.AddDays(-10), Total = 1350000, Subtotal = 1350000, Status = "Completada" };
        sale1.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p1.Variants.First(), Quantity = 1, UnitPrice = 1200000, Subtotal = 1200000 });
        sale1.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p5.Variants.First(), Quantity = 1, UnitPrice = 150000, Subtotal = 150000 });

        var sale2 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c2, Date = DateTime.UtcNow.AddDays(-2), Total = 750000, Subtotal = 750000, Status = "Completada" };
        sale2.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p3.Variants.First(), Quantity = 1, UnitPrice = 750000, Subtotal = 750000 });

        var sale3 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c3, Date = DateTime.UtcNow, Total = 1050000, Subtotal = 1050000, Status = "Completada" };
        sale3.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p2.Variants.First(), Quantity = 1, UnitPrice = 1050000, Subtotal = 1050000 });

        var sale4 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c4, Date = DateTime.UtcNow.AddDays(-1), Total = 680000, Subtotal = 680000, Status = "Completada" };
        sale4.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p4.Variants.First(), Quantity = 1, UnitPrice = 680000, Subtotal = 680000 });

        _context.Sales.AddRange(sale1, sale2, sale3, sale4);

        // Cuentas por Cobrar
        var cx1 = new AccountsReceivable { TenantId = tId, Customer = c1, SaleId = sale1.Id, OriginalAmount = 1350000, BalanceDue = 350000, IssueDate = sale1.Date, DueDate = sale1.Date.AddDays(30), Status = "PartiallyPaid" };
        var cx2 = new AccountsReceivable { TenantId = tId, Customer = c2, SaleId = sale2.Id, OriginalAmount = 750000, BalanceDue = 750000, IssueDate = sale2.Date, DueDate = sale2.Date.AddDays(15), Status = "Pending" };
        _context.AccountsReceivables.AddRange(cx1, cx2);

        await _context.SaveChangesAsync();
        return tId;
    }

    private async Task<Guid> SeedFreshMarketAsync()
    {
        var tenant = new Tenant { Name = "Fresh Market S.A.", TaxId = "3-101-777777", Email = "contacto@freshmarket.cr", Phone = "2555-5555", IsActive = true };
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        var tId = tenant.Id;

        await CreateTenantAdminAsync("admin@freshmarket.cr", tId, "Admin", "FreshMarket");

        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == "Basic");
        _context.Licenses.Add(new License { TenantId = tId, SubscriptionPlanId = plan?.Id ?? Guid.Empty, LicenseKey = Guid.NewGuid().ToString("N").ToUpper()[..16], IsActive = true, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1) });

        var b1 = new Branch { TenantId = tId, Name = "Supermercado Sabana", Address = "Sabana Norte", IsActive = true };
        _context.Branches.Add(b1);
        await _context.SaveChangesAsync();

        var cr1 = new CashRegister { TenantId = tId, BranchId = b1.Id, Name = "Caja Rápida 1", IsActive = true, IsOpen = true };
        var cr2 = new CashRegister { TenantId = tId, BranchId = b1.Id, Name = "Caja Regular 2", IsActive = true };
        var cr3 = new CashRegister { TenantId = tId, BranchId = b1.Id, Name = "Caja Regular 3", IsActive = true };
        _context.CashRegisters.AddRange(cr1, cr2, cr3);

        var adminUser = await _userManager.FindByEmailAsync("admin@freshmarket.cr");
        var session = new CashRegisterSession { TenantId = tId, CashRegister = cr1, OpenedByUserId = adminUser!.Id, OpenedAt = DateTime.UtcNow.AddHours(-8), InitialAmount = 100000 };
        _context.CashRegisterSessions.Add(session);

        var catBebidas = new Category { TenantId = tId, Name = "Bebidas", IsActive = true };
        var catLacteos = new Category { TenantId = tId, Name = "Lácteos", IsActive = true };
        var catCarnes = new Category { TenantId = tId, Name = "Carnes", IsActive = true };
        var catLimpieza = new Category { TenantId = tId, Name = "Limpieza", IsActive = true };
        _context.Categories.AddRange(catBebidas, catLacteos, catCarnes, catLimpieza);

        var p1 = new Product { TenantId = tId, Category = catBebidas, Name = "Coca Cola 3L", IsActive = true };
        p1.Variants.Add(new ProductVariant { TenantId = tId, Sku = "CC-3L", Cost = 1500, Price = 2200, IsActive = true });
        
        var p2 = new Product { TenantId = tId, Category = catBebidas, Name = "Agua Mineral 1L", IsActive = true };
        p2.Variants.Add(new ProductVariant { TenantId = tId, Sku = "AGUA-1L", Cost = 500, Price = 800, IsActive = true });
        
        var p3 = new Product { TenantId = tId, Category = catLacteos, Name = "Leche Dos Pinos 1L", IsActive = true };
        p3.Variants.Add(new ProductVariant { TenantId = tId, Sku = "LECH-1L", Cost = 800, Price = 1050, IsActive = true });

        var p4 = new Product { TenantId = tId, Category = catCarnes, Name = "Filet Mignon (kg)", IsActive = true };
        p4.Variants.Add(new ProductVariant { TenantId = tId, Sku = "CAR-FM", Cost = 12000, Price = 18000, IsActive = true });

        var p5 = new Product { TenantId = tId, Category = catLimpieza, Name = "Cloro 1L", IsActive = true };
        p5.Variants.Add(new ProductVariant { TenantId = tId, Sku = "CLOR-1L", Cost = 600, Price = 900, IsActive = true });

        _context.Products.AddRange(p1, p2, p3, p4, p5);

        var c1 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "Cliente Genérico", Identification = "0", IsActive = true };
        var c2 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "Restaurante La Parrilla", Identification = "3-101-999999", IsActive = true };
        _context.Customers.AddRange(c1, c2);

        var s1 = new Supplier { TenantId = tId, Name = "Dos Pinos", TaxId = "3-101-DPDPDP", IsActive = true };
        var s2 = new Supplier { TenantId = tId, Name = "Distribuidora Florida", TaxId = "3-101-FLFLFL", IsActive = true };
        var s3 = new Supplier { TenantId = tId, Name = "Carnes El Arreo", TaxId = "3-101-CARNES", IsActive = true };
        _context.Suppliers.AddRange(s1, s2, s3);

        await _context.SaveChangesAsync();

        var sale1 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c1, Date = DateTime.UtcNow, Total = 4050, Subtotal = 4050, Status = "Completada" };
        sale1.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p1.Variants.First(), Quantity = 1, UnitPrice = 2200, Subtotal = 2200 });
        sale1.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p2.Variants.First(), Quantity = 1, UnitPrice = 800, Subtotal = 800 });
        sale1.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p3.Variants.First(), Quantity = 1, UnitPrice = 1050, Subtotal = 1050 });
        
        var sale2 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c2, Date = DateTime.UtcNow.AddDays(-1), Total = 54000, Subtotal = 54000, Status = "Completada" };
        sale2.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p4.Variants.First(), Quantity = 3, UnitPrice = 18000, Subtotal = 54000 });

        var sale3 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c1, Date = DateTime.UtcNow.AddDays(-2), Total = 1800, Subtotal = 1800, Status = "Completada" };
        sale3.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p5.Variants.First(), Quantity = 2, UnitPrice = 900, Subtotal = 1800 });
        
        _context.Sales.AddRange(sale1, sale2, sale3);
        await _context.SaveChangesAsync();
        return tId;
    }

    private async Task<Guid> SeedEliteServicesAsync()
    {
        var tenant = new Tenant { Name = "Elite Consulting Services", TaxId = "3-101-888888", Email = "contact@eliteservices.cr", IsActive = true };
        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();
        var tId = tenant.Id;

        await CreateTenantAdminAsync("admin@eliteservices.cr", tId, "Admin", "EliteServices");

        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == "Basic");
        _context.Licenses.Add(new License { TenantId = tId, SubscriptionPlanId = plan?.Id ?? Guid.Empty, LicenseKey = Guid.NewGuid().ToString("N").ToUpper()[..16], IsActive = true, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddYears(1) });

        var b1 = new Branch { TenantId = tId, Name = "Oficina Principal", Address = "Torre Mercedes", IsActive = true };
        _context.Branches.Add(b1);
        await _context.SaveChangesAsync();

        var cr1 = new CashRegister { TenantId = tId, BranchId = b1.Id, Name = "Facturación Corporativa", IsActive = true, IsOpen = true };
        _context.CashRegisters.Add(cr1);

        var adminUser = await _userManager.FindByEmailAsync("admin@eliteservices.cr");
        var session = new CashRegisterSession { TenantId = tId, CashRegister = cr1, OpenedByUserId = adminUser!.Id, OpenedAt = DateTime.UtcNow.AddHours(-8), InitialAmount = 0 };
        _context.CashRegisterSessions.Add(session);

        var catConsul = new Category { TenantId = tId, Name = "Consultoría", IsActive = true };
        var catSoporte = new Category { TenantId = tId, Name = "Soporte TI", IsActive = true };
        var catCapa = new Category { TenantId = tId, Name = "Capacitación", IsActive = true };
        _context.Categories.AddRange(catConsul, catSoporte, catCapa);

        var p1 = new Product { TenantId = tId, Category = catConsul, Name = "Auditoría de Seguridad", IsActive = true };
        p1.Variants.Add(new ProductVariant { TenantId = tId, Sku = "AUD-SEG", Cost = 0, Price = 500000, IsActive = true });
        
        var p2 = new Product { TenantId = tId, Category = catSoporte, Name = "Horas Soporte Servidores", IsActive = true };
        p2.Variants.Add(new ProductVariant { TenantId = tId, Sku = "SOP-HR", Cost = 0, Price = 35000, IsActive = true });

        var p3 = new Product { TenantId = tId, Category = catCapa, Name = "Curso Ciberseguridad", IsActive = true };
        p3.Variants.Add(new ProductVariant { TenantId = tId, Sku = "CUR-CIB", Cost = 0, Price = 250000, IsActive = true });

        _context.Products.AddRange(p1, p2, p3);

        var c1 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "Banco Nacional", Identification = "4-000-000000", IsActive = true };
        var c2 = new NexusERP.Domain.Entities.Sales.Customer { TenantId = tId, Name = "Cooperativa X", Identification = "3-000-000000", IsActive = true };
        _context.Customers.AddRange(c1, c2);

        await _context.SaveChangesAsync();

        var sale1 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c1, Date = DateTime.UtcNow.AddDays(-5), Total = 500000, Subtotal = 500000, Status = "Completada" };
        sale1.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p1.Variants.First(), Quantity = 1, UnitPrice = 500000, Subtotal = 500000 });
        _context.Sales.Add(sale1);

        var sale2 = new NexusERP.Domain.Entities.Sales.Sale { TenantId = tId, Branch = b1, Session = session, Customer = c2, Date = DateTime.UtcNow.AddDays(-1), Total = 750000, Subtotal = 750000, Status = "Completada" };
        sale2.Details.Add(new NexusERP.Domain.Entities.Sales.SaleDetail { TenantId = tId, ProductVariant = p3.Variants.First(), Quantity = 3, UnitPrice = 250000, Subtotal = 750000 });
        _context.Sales.Add(sale2);

        var cx1 = new AccountsReceivable { TenantId = tId, Customer = c1, SaleId = sale1.Id, OriginalAmount = 500000, BalanceDue = 500000, IssueDate = sale1.Date, DueDate = sale1.Date.AddDays(45), Status = "Pending" };
        var cx2 = new AccountsReceivable { TenantId = tId, Customer = c2, SaleId = sale2.Id, OriginalAmount = 750000, BalanceDue = 750000, IssueDate = sale2.Date, DueDate = sale2.Date.AddDays(30), Status = "Pending" };
        _context.AccountsReceivables.AddRange(cx1, cx2);

        await _context.SaveChangesAsync();
        return tId;
    }

    private async Task CreateTenantAdminAsync(string email, Guid tenantId, string firstName, string lastName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                TenantId = tenantId
            };

            var result = await _userManager.CreateAsync(user, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "TenantAdmin");
            }
        }
        else
        {
            user.TenantId = tenantId;
            await _userManager.UpdateAsync(user);
        }
    }
}
