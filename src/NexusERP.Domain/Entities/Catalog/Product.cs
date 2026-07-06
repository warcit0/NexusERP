namespace NexusERP.Domain.Entities.Catalog;

public class Product : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Clasificación y Catálogo
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    
    public Guid? BrandId { get; set; }
    public Brand? Brand { get; set; }
    
    public Guid? TaxId { get; set; }
    public Tax? Tax { get; set; }
    
    // Datos específicos (ej: Costa Rica CABYS)
    public string CabysCode { get; set; } = string.Empty;
    public string MeasurementUnit { get; set; } = "Unidad"; // Ej: Unidad, Kg, L
    
    public bool IsActive { get; set; } = true;
    
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
}
