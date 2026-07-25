using NexusERP.Domain.Entities;
using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Domain.Entities.Finance;

public class AccountsReceivable : TenantEntity
{
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public Guid SaleId { get; set; } // La venta que originó la deuda
    
    public string InvoiceNumber { get; set; } = string.Empty; // Ej: TKT-0001
    
    public decimal OriginalAmount { get; set; }
    public decimal BalanceDue { get; set; }
    
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    
    // Pending, PartiallyPaid, Paid, Overdue
    public string Status { get; set; } = "Pending";
    
    public List<CustomerPayment> Payments { get; set; } = new();
}
