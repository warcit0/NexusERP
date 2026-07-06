using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Domain.Entities.Inventory;

public class InventoryTransaction : TenantEntity
{
    public Guid ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public string TransactionType { get; set; } = string.Empty; // "Entrada", "Salida", "Ajuste"
    public decimal Quantity { get; set; } // Puede ser negativo o positivo dependiendo de la semántica
    
    public string Reference { get; set; } = string.Empty; // "Venta #0001", "Ajuste Manual", "Compra #992"
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
}
