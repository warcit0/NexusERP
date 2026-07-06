using MediatR;

namespace NexusERP.Application.Settings.Branches.Commands.CreateBranch;

public record CreateBranchCommand(string Name, string Code, string Address, string Phone) : IRequest<Guid>;
