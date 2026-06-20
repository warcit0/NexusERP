using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;

namespace NexusERP.Application.Platform.Licenses.Commands.ActivateLicense;

public class ActivateLicenseCommandHandler : IRequestHandler<ActivateLicenseCommand, ActivateLicenseResponse>
{
    private readonly INexusDbContext _context;
    private readonly ILicenseSettings _licenseSettings;

    public ActivateLicenseCommandHandler(INexusDbContext context, ILicenseSettings licenseSettings)
    {
        _context = context;
        _licenseSettings = licenseSettings;
    }

    public async Task<ActivateLicenseResponse> Handle(ActivateLicenseCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar firma HMAC-SHA256 para prevenir manipulaciones
        var expectedSignature = ComputeHmacSha256(
            $"{request.TenantId}:{request.ActivationCode}",
            _licenseSettings.HmacSecret);

        if (!expectedSignature.Equals(request.Signature, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("Firma de activación inválida.");
        }

        // 2. Buscar el Tenant
        var tenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken)
            ?? throw new InvalidOperationException($"Tenant {request.TenantId} no encontrado.");

        // 3. Buscar el plan asociado al código de activación (simplificado: el código es el PlanCode)
        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Code == request.ActivationCode, cancellationToken)
            ?? throw new InvalidOperationException($"Plan '{request.ActivationCode}' no encontrado.");

        // 4. Crear la licencia
        var license = new License
        {
            TenantId = request.TenantId,
            SubscriptionPlanId = plan.Id,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(plan.DurationDays),
            IsActive = true
        };

        _context.Licenses.Add(license);

        // 5. Actualizar el plan del tenant
        tenant.CurrentSubscriptionPlanId = plan.Id;

        await _context.SaveChangesAsync(cancellationToken);

        return new ActivateLicenseResponse
        {
            LicenseId = license.Id,
            TenantId = request.TenantId,
            ExpiresAt = license.EndDate,
            PlanName = plan.Name
        };
    }

    private static string ComputeHmacSha256(string data, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA256(keyBytes);
        var hash = hmac.ComputeHash(dataBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
