using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Infrastructure.Persistence;
using NexusERP.Infrastructure.Persistence.Interceptors;

namespace NexusERP.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<NexusDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(NexusDbContext).Assembly.FullName));
        });

        services.AddScoped<INexusDbContext>(provider => provider.GetRequiredService<NexusDbContext>());

        services.AddIdentityCore<NexusERP.Infrastructure.Identity.ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
        .AddEntityFrameworkStores<NexusDbContext>();

        // Se configurarán ICurrentUserService y ICurrentTenantService en la API (ya que dependen de HttpContext)
        // services.AddTransient<IDateTime, DateTimeService>(); // TODO: Servicio de fecha

        return services;
    }
}
