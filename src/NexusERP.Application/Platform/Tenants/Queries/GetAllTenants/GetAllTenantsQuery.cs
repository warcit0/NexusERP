using MediatR;

namespace NexusERP.Application.Platform.Tenants.Queries.GetAllTenants;

public record GetAllTenantsQuery : IRequest<List<TenantDto>>;

public record TenantDto(
    Guid Id,
    string Name,
    string Subdomain,
    string Status,
    bool IsActive,
    string PlanName,
    DateTime CreatedAt,
    int UserCount
);
