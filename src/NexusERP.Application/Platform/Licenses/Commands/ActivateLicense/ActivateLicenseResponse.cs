namespace NexusERP.Application.Platform.Licenses.Commands.ActivateLicense;

public class ActivateLicenseResponse
{
    public Guid LicenseId { get; set; }
    public Guid TenantId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string PlanName { get; set; } = string.Empty;
}
