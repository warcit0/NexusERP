namespace NexusERP.Domain.Entities;

public class CashRegister : TenantEntity
{
    public string Name { get; set; } = string.Empty; // Ej. Caja 01
    public string MacAddress { get; set; } = string.Empty; // Para restringir acceso físico
    public bool IsActive { get; set; } = true;
    public bool IsOpen { get; set; } = false;
    
    public Guid BranchId { get; set; }
    public Branch? Branch { get; set; }
}
