using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace NexusERP.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IDistributedCache _cache;

    public LogoutCommandHandler(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // En una implementación real, extraeríamos el tiempo de vida (expiration) 
        // restante del JWT para que la key en caché expire al mismo tiempo que el token.
        // Aquí configuramos un tiempo máximo por defecto igual al de la sesión (ej. 60 min)
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
        };

        await _cache.SetStringAsync($"blacklist:{request.AccessToken}", "revoked", options, cancellationToken);
    }
}
