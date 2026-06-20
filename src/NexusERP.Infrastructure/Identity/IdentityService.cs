using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using NexusERP.Application.Auth.Commands.Login;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<(bool Result, AuthResponse? AuthResponse)> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return (false, null);

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            return (false, null);

        var userRoles = await _userManager.GetRolesAsync(user);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
        };

        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));
        }

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // En un sistema real se debería guardar el RefreshToken en la BD asociado al usuario.
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Leer de settings preferiblemente
        await _userManager.UpdateAsync(user);

        return (true, new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Email = user.Email!,
            UserId = user.Id,
            TenantId = user.TenantId,
            Roles = userRoles.ToList()
        });
    }

    public async Task<string?> CreateUserAsync(string email, string password, Guid? tenantId)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            TenantId = tenantId
        };

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded ? user.Id : null;
    }

    public async Task<bool> AddUserToRoleAsync(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded;
    }

    public async Task<AuthResponse?> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        if (email == null) return null;

        var user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Preferible leer de JwtSettings
        await _userManager.UpdateAsync(user);

        return new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            Email = user.Email!,
            UserId = user.Id,
            TenantId = user.TenantId,
            Roles = (await _userManager.GetRolesAsync(user)).ToList()
        };
    }
}
