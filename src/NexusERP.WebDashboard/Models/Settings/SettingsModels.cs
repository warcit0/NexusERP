namespace NexusERP.WebDashboard.Models.Settings;

// Duplicado temporal para resolver dependencias rápidas de Stock.razor y POS.razor
public class BranchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CashRegisterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Abierta", "Cerrada"
    public bool IsOpen { get; set; }
}

