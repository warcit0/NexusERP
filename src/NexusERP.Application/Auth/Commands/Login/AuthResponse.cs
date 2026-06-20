namespace NexusERP.Application.Auth.Commands.Login;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
