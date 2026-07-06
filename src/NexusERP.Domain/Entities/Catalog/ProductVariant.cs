namespace NexusERP.Domain.Entities.Catalog;

public class ProductVariant : TenantEntity
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    public string Sku { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    
    // Atributos de Variante (Talla, Color)
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    
    // Precios
    public decimal Cost { get; set; }
    public decimal Price { get; set; }
    
    public bool IsActive { get; set; } = true;
}
