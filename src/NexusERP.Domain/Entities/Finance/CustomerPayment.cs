using NexusERP.Domain.Entities;

namespace NexusERP.Domain.Entities.Finance;

public class CustomerPayment : TenantEntity
{
    public Guid AccountsReceivableId { get; set; }
    public AccountsReceivable? AccountsReceivable { get; set; }
    
    public DateTime PaymentDate { get; set; }
    
    public decimal Amount { get; set; }
    
    public string PaymentMethod { get; set; } = string.Empty; // Cash, Card, Transfer
    public string Reference { get; set; } = string.Empty; // Ej: Comprobante transferencia
    
    public Guid? CashRegisterSessionId { get; set; } // Si ingresó por caja
    public string Notes { get; set; } = string.Empty;
}
