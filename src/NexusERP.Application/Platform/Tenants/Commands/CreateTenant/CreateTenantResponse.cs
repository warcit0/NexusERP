namespace NexusERP.Application.Platform.Tenants.Commands.CreateTenant;

public class CreateTenantResponse
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Subdomain { get; set; } = string.Empty;
    public string AdminUserId { get; set; } = string.Empty;
}
