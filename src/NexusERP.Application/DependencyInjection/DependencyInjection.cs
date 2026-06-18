using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace NexusERP.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // Aquí agregaremos Behaviors (Validation, Performance, Logging) en el futuro
        });

        return services;
    }
}
