using Microsoft.AspNetCore.Identity;

namespace NexusERP.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? TenantId { get; set; } // Null si es SuperAdmin de la plataforma
}
