namespace NexusERP.Domain.Entities.Catalog;

public class Brand : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
