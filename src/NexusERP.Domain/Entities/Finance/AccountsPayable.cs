using NexusERP.Domain.Entities;
using NexusERP.Domain.Entities.Purchases;

namespace NexusERP.Domain.Entities.Finance;

public class AccountsPayable : TenantEntity
{
    public Guid SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    
    public Guid? PurchaseOrderId { get; set; } // Opcional, puede ser un gasto sin orden
    
    public string InvoiceNumber { get; set; } = string.Empty; // Factura del proveedor
    
    public decimal OriginalAmount { get; set; }
    public decimal BalanceDue { get; set; }
    
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    
    // Pending, PartiallyPaid, Paid, Overdue
    public string Status { get; set; } = "Pending";
    
    public List<SupplierPayment> Payments { get; set; } = new();
}
