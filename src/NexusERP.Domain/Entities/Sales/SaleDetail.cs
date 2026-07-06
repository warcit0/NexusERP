using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Domain.Entities.Sales;

public class SaleDetail : TenantEntity
{
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
}
