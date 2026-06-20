using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Infrastructure.Persistence;
using NexusERP.Infrastructure.Persistence.Interceptors;
using Microsoft.Extensions.Caching.Distributed;

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

        services.AddIdentity<NexusERP.Infrastructure.Identity.ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<NexusDbContext>();

        services.Configure<NexusERP.Infrastructure.Identity.JwtSettings>(configuration.GetSection(NexusERP.Infrastructure.Identity.JwtSettings.SectionName));
        services.AddScoped<ITokenService, NexusERP.Infrastructure.Identity.TokenService>();
        services.AddScoped<IIdentityService, NexusERP.Infrastructure.Identity.IdentityService>();

        var jwtSettings = new NexusERP.Infrastructure.Identity.JwtSettings();
        configuration.Bind(NexusERP.Infrastructure.Identity.JwtSettings.SectionName, jwtSettings);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };

            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var cache = context.HttpContext.RequestServices.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
                    var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                    
                    var isRevoked = await cache.GetStringAsync($"blacklist:{token}");
                    if (!string.IsNullOrEmpty(isRevoked))
                    {
                        context.Fail("Token has been revoked");
                    }
                }
            };
        });

        // Se configurarán ICurrentUserService y ICurrentTenantService en la API (ya que dependen de HttpContext)
        // services.AddTransient<IDateTime, DateTimeService>(); // TODO: Servicio de fecha

        return services;
    }
}
