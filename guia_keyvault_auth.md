# Guía: Configuración de Azure Key Vault y Políticas de Autorización

Esta guía explica los pasos necesarios para completar la configuración de integración con Azure Key Vault y las políticas de autorización básicas en NexusERP.

## 1. Integración con Azure Key Vault

Azure Key Vault nos permite almacenar secretos de forma segura (como Connection Strings y la clave secreta de JWT) fuera del código fuente o variables de entorno inseguras.

### Requisitos previos en Azure
1. Tener creado un recurso Key Vault en Azure.
2. Otorgar permisos al App Service (o a tu usuario local de Azure CLI) para acceder a los secretos (usando RBAC: "Key Vault Secrets User").
3. Agregar los secretos necesarios en el Key Vault (ej. `ConnectionStrings--DefaultConnection`, `JwtSettings--Secret`).

### Implementación en el Código (Ya implementada en `Program.cs`)
En `NexusERP.API/Program.cs`, utilizamos el paquete `Azure.Identity` para inyectar automáticamente los secretos como configuraciones de .NET durante entornos de Producción.

```csharp
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVaultUri"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new Azure.Identity.DefaultAzureCredential());
    }
}
```

### Configuración local (Desarrollo)
En entorno de desarrollo local, **no** uses el Key Vault si no tienes conexión a Azure. Utiliza los `User Secrets` de .NET:
```bash
cd src/NexusERP.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=..."
dotnet user-secrets set "JwtSettings:Secret" "TuSecretoFuerteLocalDePrueba"
```

---

## 2. Establecer Políticas de Autorización Básicas

ASP.NET Core permite definir políticas de autorización basadas en Claims, Roles o Requisitos personalizados (Policy-based authorization).

### Definición de Políticas (En `Program.cs`)
Actualmente hemos llamado a `builder.Services.AddAuthorization()`. Para definir políticas específicas, modifica la configuración de esta manera:

```csharp
builder.Services.AddAuthorization(options =>
{
    // Política para requerir que el usuario sea SuperAdmin (Platform Level)
    options.AddPolicy("RequireSuperAdmin", policy =>
        policy.RequireRole("SuperAdmin"));

    // Política para verificar que el usuario pertenece a un Tenant específico
    options.AddPolicy("TenantAccess", policy =>
        policy.RequireClaim("TenantId"));

    // Política por Módulo/Permiso
    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireClaim("Permission", "Users.Manage"));
});
```

### Uso de Políticas en Controladores
Una vez definidas, puedes proteger tus endpoints con el atributo `[Authorize]`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Solo usuarios autenticados que cumplan la política CanManageUsers
    [Authorize(Policy = "CanManageUsers")]
    [HttpPost]
    public IActionResult CreateUser()
    {
        return Ok();
    }

    // Solo Super Administradores
    [Authorize(Policy = "RequireSuperAdmin")]
    [HttpGet("system-logs")]
    public IActionResult GetLogs()
    {
        return Ok();
    }
}
```

### Asignación de Roles y Claims
Cuando se genere el token JWT en el módulo de Autenticación, asegúrate de incluir los Claims necesarios:

```csharp
var claims = new List<Claim>
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim("TenantId", user.TenantId?.ToString() ?? ""),
    new Claim(ClaimTypes.Role, "SuperAdmin") // Si aplica
};
```
