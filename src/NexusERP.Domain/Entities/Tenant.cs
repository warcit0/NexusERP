namespace NexusERP.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string CommercialName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty; // Cédula o RUC
    public string Subdomain { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    
    // Configuración de la Suscripción
    public bool IsActive { get; set; } = true;
    public Guid? CurrentSubscriptionPlanId { get; set; }
    public SubscriptionPlan? CurrentSubscriptionPlan { get; set; }
}
