using NexusERP.Domain.Entities;

namespace NexusERP.Domain.Entities.Finance;

public class SupplierPayment : TenantEntity
{
    public Guid AccountsPayableId { get; set; }
    public AccountsPayable? AccountsPayable { get; set; }
    
    public DateTime PaymentDate { get; set; }
    
    public decimal Amount { get; set; }
    
    public string PaymentMethod { get; set; } = string.Empty; // Transfer, Check, Cash
    public string Reference { get; set; } = string.Empty;
    
    public Guid? CashRegisterSessionId { get; set; } // Si salió de la caja
    public string Notes { get; set; } = string.Empty;
}
