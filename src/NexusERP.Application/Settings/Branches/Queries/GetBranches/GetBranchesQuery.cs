using MediatR;

namespace NexusERP.Application.Settings.Branches.Queries.GetBranches;

public record BranchDto(Guid Id, string Name, string Code, string Address, string Phone, bool IsActive);

public record GetBranchesQuery : IRequest<List<BranchDto>>;
