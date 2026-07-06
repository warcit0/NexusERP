using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexusERP.Infrastructure.Identity;
using NexusERP.Domain.Entities;

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
        // 1. Seed Roles
        var roles = new[] { "SuperAdmin", "TenantAdmin", "User" };
        foreach (var role in roles)
        {
            if (await _roleManager.FindByNameAsync(role) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // 2. Seed Subscription Plans (opcional para no dejarlo vacío)
        if (!await _context.SubscriptionPlans.AnyAsync())
        {
            _context.SubscriptionPlans.AddRange(
                new SubscriptionPlan { Name = "Basic", MaxUsers = 5, MaxBranches = 1, MonthlyPrice = 29.99m, AnnualPrice = 299.99m },
                new SubscriptionPlan { Name = "Pro", MaxUsers = 20, MaxBranches = 5, MonthlyPrice = 99.99m, AnnualPrice = 999.99m }
            );
            await _context.SaveChangesAsync();
        }

        // 3. Seed SuperAdmin User
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
                // TenantId is null for platform super admins
            };

            var result = await _userManager.CreateAsync(superAdmin, "Admin123!");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }
        }
    }
}
