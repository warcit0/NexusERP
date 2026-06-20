using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace NexusERP.WebDashboard.Auth;

/// <summary>
/// Proveedor personalizado de estado de autenticación para Blazor WASM.
/// Lee el JWT desde localStorage y extrae los claims del token.
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _http;
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
    {
        _localStorage = localStorage;
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrWhiteSpace(token))
            return _anonymous;

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        // Inyectar el token en el HttpClient para llamadas autenticadas
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return new AuthenticationState(user);
    }

    public async Task NotifyUserAuthenticated(string token, string refreshToken)
    {
        await _localStorage.SetItemAsync("authToken", token);
        await _localStorage.SetItemAsync("refreshToken", refreshToken);

        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task NotifyUserLoggedOut()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("refreshToken");

        _http.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();

        try
        {
            var payload = jwt.Split('.')[1];
            // Pad base64 string
            var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            var jsonBytes = Convert.FromBase64String(padded);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);

            if (keyValuePairs == null) return claims;

            // Mapear claims estándar
            if (keyValuePairs.TryGetValue(ClaimTypes.NameIdentifier, out var nameId))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId.GetString() ?? ""));

            if (keyValuePairs.TryGetValue(ClaimTypes.Email, out var email))
                claims.Add(new Claim(ClaimTypes.Email, email.GetString() ?? ""));

            if (keyValuePairs.TryGetValue(ClaimTypes.Name, out var name))
                claims.Add(new Claim(ClaimTypes.Name, name.GetString() ?? ""));

            if (keyValuePairs.TryGetValue("TenantId", out var tenantId))
                claims.Add(new Claim("TenantId", tenantId.GetString() ?? ""));

            // Roles pueden ser un array o un string
            if (keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles))
            {
                if (roles.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in roles.EnumerateArray())
                        claims.Add(new Claim(ClaimTypes.Role, role.GetString() ?? ""));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.GetString() ?? ""));
                }
            }
        }
        catch
        {
            // Token inválido — devolver vacío
        }

        return claims;
    }
}

/// <summary>
/// Servicio simple de LocalStorage para Blazor WASM vía JSInterop.
/// </summary>
public interface ILocalStorageService
{
    Task<T?> GetItemAsync<T>(string key);
    Task SetItemAsync<T>(string key, T value);
    Task RemoveItemAsync(string key);
}

public class LocalStorageService : ILocalStorageService
{
    private readonly Microsoft.JSInterop.IJSRuntime _js;
    public LocalStorageService(Microsoft.JSInterop.IJSRuntime js) => _js = js;

    public async Task<T?> GetItemAsync<T>(string key)
    {
        var json = await Microsoft.JSInterop.JSRuntimeExtensions.InvokeAsync<string?>(_js, "localStorage.getItem", key);
        if (json == null) return default;
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItemAsync<T>(string key, T value)
        => await Microsoft.JSInterop.JSRuntimeExtensions.InvokeVoidAsync(_js, "localStorage.setItem", key, JsonSerializer.Serialize(value));

    public async Task RemoveItemAsync(string key)
        => await Microsoft.JSInterop.JSRuntimeExtensions.InvokeVoidAsync(_js, "localStorage.removeItem", key);
}
