namespace NexusERP.Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // Código único del plan (ej: "BASIC", "PRO")
    public decimal MonthlyPrice { get; set; }
    public decimal AnnualPrice { get; set; }
    public int DurationDays { get; set; } = 30; // Duración por defecto: 30 días
    
    // Límites del plan
    public int MaxUsers { get; set; }
    public int MaxBranches { get; set; }
    public int MaxInvoicesPerMonth { get; set; }
    public bool IncludesAdvancedAnalytics { get; set; }
    
    public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
}
