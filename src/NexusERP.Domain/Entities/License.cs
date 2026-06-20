namespace NexusERP.Domain.Entities;

public class License : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant? Tenant { get; set; }
    
    public string LicenseKey { get; set; } = string.Empty; // Hash de la licencia
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
    
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan? SubscriptionPlan { get; set; }
}
