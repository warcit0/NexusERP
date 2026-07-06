using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Domain.Entities.Sales;

public class Sale : TenantEntity
{
    public Guid CashRegisterSessionId { get; set; }
    public CashRegisterSession Session { get; set; } = null!;

    public Guid BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public decimal Subtotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }

    // Estado: "Completada", "Anulada"
    public string Status { get; set; } = "Completada";

    public ICollection<SaleDetail> Details { get; set; } = new List<SaleDetail>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
