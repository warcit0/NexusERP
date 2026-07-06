namespace NexusERP.Domain.Entities.Sales;

public class CashRegisterSession : TenantEntity
{
    public Guid CashRegisterId { get; set; }
    public CashRegister? CashRegister { get; set; }
    
    public string OpenedByUserId { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; }
    public decimal InitialAmount { get; set; }
    
    public string? ClosedByUserId { get; set; }
    public DateTime? ClosedAt { get; set; }
    public decimal? FinalAmount { get; set; }
    public decimal? ExpectedAmount { get; set; }
    
    public string Notes { get; set; } = string.Empty;
    
    // Estado
    public bool IsClosed { get; set; } = false;
}
