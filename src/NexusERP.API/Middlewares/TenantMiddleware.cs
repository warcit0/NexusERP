using System.Security.Claims;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.API.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantService currentTenantService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = context.User.FindFirstValue("TenantId");
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                currentTenantService.SetTenant(tenantId);
            }
        }

        await _next(context);
    }
}
