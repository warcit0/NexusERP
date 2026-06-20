using NexusERP.Application.Auth.Commands.Login;

namespace NexusERP.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Result, AuthResponse? AuthResponse)> LoginAsync(string email, string password);
    Task<string?> CreateUserAsync(string email, string password, Guid? tenantId);
    Task<bool> AddUserToRoleAsync(string userId, string roleName);
    Task<AuthResponse?> RefreshTokenAsync(string accessToken, string refreshToken);
}
