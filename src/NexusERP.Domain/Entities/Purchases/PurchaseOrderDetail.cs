using NexusERP.Domain.Entities;
using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Domain.Entities.Purchases;

public class PurchaseOrderDetail : TenantEntity
{
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }
    
    public Guid ProductVariantId { get; set; }
    public ProductVariant? ProductVariant { get; set; }
    
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    
    public decimal QuantityOrdered { get; set; }
    public decimal QuantityReceived { get; set; }
    
    public decimal UnitCost { get; set; }
    public decimal Subtotal { get; set; }
}
