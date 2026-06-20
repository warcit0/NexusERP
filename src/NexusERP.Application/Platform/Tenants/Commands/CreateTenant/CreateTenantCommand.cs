using MediatR;

namespace NexusERP.Application.Platform.Tenants.Commands.CreateTenant;

public record CreateTenantCommand(
    string Name,
    string Subdomain,
    string AdminEmail,
    string AdminPassword,
    string PlanCode
) : IRequest<CreateTenantResponse>;
