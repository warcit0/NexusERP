namespace NexusERP.Domain.Entities.Catalog;

public class Tax : TenantEntity
{
    public string Name { get; set; } = string.Empty; // Ej: IVA 13%
    public decimal Percentage { get; set; } // Ej: 13.00
    public bool IsActive { get; set; } = true;
}
