using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Platform.Tenants.Commands.UpdateTenant;

public record UpdateTenantCommand(Guid Id, string Name, string CommercialName, string TaxId, string Subdomain, string Email, string Phone, bool IsActive) : IRequest<bool>;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, bool>
{
    private readonly INexusDbContext _context;

    public UpdateTenantCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
        if (tenant == null) return false;

        tenant.Name = request.Name;
        tenant.CommercialName = request.CommercialName;
        tenant.TaxId = request.TaxId;
        tenant.Subdomain = request.Subdomain;
        tenant.Email = request.Email;
        tenant.Phone = request.Phone;
        tenant.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
