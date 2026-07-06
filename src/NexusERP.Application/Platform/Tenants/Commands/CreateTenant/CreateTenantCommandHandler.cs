using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;

namespace NexusERP.Application.Platform.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, CreateTenantResponse>
{
    private readonly INexusDbContext _context;
    private readonly IIdentityService _identityService;

    public CreateTenantCommandHandler(
        INexusDbContext context,
        IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async Task<CreateTenantResponse> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // 1. Buscar el plan de suscripción si se especificó
        var plan = string.IsNullOrEmpty(request.PlanCode)
            ? null
            : await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.Name == request.PlanCode, cancellationToken);

        // 2. Crear el Tenant
        var tenant = new Tenant
        {
            Name = request.Name,
            Subdomain = string.IsNullOrEmpty(request.Subdomain)
                ? request.Name.ToLower().Replace(" ", "-")
                : request.Subdomain,
            IsActive = true,
            CurrentSubscriptionPlanId = plan?.Id
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Crear el usuario administrador del Tenant
        string? adminUserId;
        try
        {
            adminUserId = await _identityService.CreateUserAsync(
                request.AdminEmail,
                request.AdminPassword,
                tenant.Id);
        }
        catch (InvalidOperationException ex)
        {
            // Rollback del tenant si el usuario falló por reglas de Identity
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException(ex.Message);
        }

        // 3. Asignar rol de TenantAdmin
        await _identityService.AddUserToRoleAsync(adminUserId, "TenantAdmin");

        return new CreateTenantResponse
        {
            TenantId = tenant.Id,
            Name = tenant.Name,
            Subdomain = tenant.Subdomain,
            AdminUserId = adminUserId
        };
    }
}
