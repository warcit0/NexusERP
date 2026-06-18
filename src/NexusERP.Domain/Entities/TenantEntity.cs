namespace NexusERP.Domain.Entities;

/// <summary>
/// Entidad con contexto de tenant.
/// Toda entidad de negocio de un tenant hereda de esta clase.
/// </summary>
public abstract class TenantEntity : AuditableEntity
{
    public Guid TenantId { get; set; }
}
