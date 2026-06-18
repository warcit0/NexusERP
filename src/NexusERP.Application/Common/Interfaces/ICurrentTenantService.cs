namespace NexusERP.Application.Common.Interfaces;

/// <summary>
/// Servicio para resolver el tenant actual en el contexto de la solicitud HTTP (a partir del JWT).
/// </summary>
public interface ICurrentTenantService
{
    Guid? TenantId { get; }
    void SetTenant(Guid tenantId);
}
