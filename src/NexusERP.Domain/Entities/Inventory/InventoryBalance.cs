using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Domain.Entities.Inventory;

public class InventoryBalance : TenantEntity
{
    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public decimal CurrentStock { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
