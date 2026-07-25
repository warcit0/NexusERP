using NexusERP.Domain.Entities;

namespace NexusERP.Domain.Entities.Purchases;

// Proveedor
public class Supplier : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty; // Cédula o NIT
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
