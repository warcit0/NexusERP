using MediatR;
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
        // 1. Crear el Tenant
        var tenant = new Tenant
        {
            Name = request.Name,
            Subdomain = request.Subdomain,
            IsActive = true
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Crear el usuario administrador del Tenant
        var adminUserId = await _identityService.CreateUserAsync(
            request.AdminEmail,
            request.AdminPassword,
            tenant.Id);

        if (adminUserId == null)
        {
            // Rollback del tenant si el usuario no se pudo crear
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException($"No se pudo crear el usuario administrador: {request.AdminEmail}");
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
