using NexusERP.Domain.Entities;

namespace NexusERP.Domain.Entities.Purchases;

public class PurchaseOrder : TenantEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; } // Enlazado a Proveedores (se debe crear o reutilizar)
    public string SupplierName { get; set; } = string.Empty;
    public Guid BranchId { get; set; } // Dónde se va a recibir
    
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    
    // Draft, Sent, PartiallyReceived, Received, Cancelled
    public string Status { get; set; } = "Draft";
    
    public decimal TotalAmount { get; set; }
    public string Notes { get; set; } = string.Empty;

    public List<PurchaseOrderDetail> Details { get; set; } = new();
}
