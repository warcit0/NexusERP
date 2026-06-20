using MediatR;

namespace NexusERP.Application.Platform.Licenses.Commands.ActivateLicense;

/// <summary>
/// Activa una licencia para un Tenant dado un código de activación firmado con HMAC-SHA256.
/// </summary>
public record ActivateLicenseCommand(
    Guid TenantId,
    string ActivationCode,
    string Signature  // HMAC-SHA256 de TenantId+ActivationCode para prevenir manipulaciones
) : IRequest<ActivateLicenseResponse>;
