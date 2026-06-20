namespace NexusERP.Domain.Entities;

public class Branch : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Ej. SUC-001
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    public ICollection<CashRegister> CashRegisters { get; set; } = new List<CashRegister>();
}
