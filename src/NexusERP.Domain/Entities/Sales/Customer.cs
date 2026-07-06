namespace NexusERP.Domain.Entities.Sales;

public class Customer : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
