using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Infrastructure.BackgroundServices;

/// <summary>
/// Worker service que revisa periódicamente las licencias vencidas
/// y suspende los tenants cuya licencia ha expirado.
/// </summary>
public class TenantSuspensionBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TenantSuspensionBackgroundService> _logger;

    // Intervalo de revisión: cada 24 horas
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

    public TenantSuspensionBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<TenantSuspensionBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TenantSuspensionBackgroundService iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAndSuspendExpiredTenantsAsync(stoppingToken);
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckAndSuspendExpiredTenantsAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<INexusDbContext>();

            var now = DateTime.UtcNow;

            // Buscar licencias activas que ya vencieron
            var expiredLicenses = await dbContext.Licenses
                .Where(l => l.IsActive && l.EndDate < now)
                .Include(l => l.Tenant)
                .ToListAsync(cancellationToken);

            if (!expiredLicenses.Any())
            {
                _logger.LogDebug("No se encontraron licencias vencidas en la revisión.");
                return;
            }

            _logger.LogInformation("Encontradas {Count} licencias vencidas. Suspendiendo tenants...", expiredLicenses.Count);

            foreach (var license in expiredLicenses)
            {
                // Desactivar la licencia
                license.IsActive = false;

                // Suspender el tenant
                if (license.Tenant != null)
                {
                    license.Tenant.IsActive = false;
                    _logger.LogWarning("Tenant '{TenantName}' (ID: {TenantId}) suspendido por licencia vencida el {ExpiredAt}.",
                        license.Tenant.Name, license.Tenant.Id, license.EndDate);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Graceful shutdown
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al revisar licencias vencidas.");
        }
    }
}
