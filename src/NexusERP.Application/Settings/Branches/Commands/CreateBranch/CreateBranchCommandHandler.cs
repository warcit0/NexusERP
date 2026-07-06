using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;

namespace NexusERP.Application.Settings.Branches.Commands.CreateBranch;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateBranchCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
    {
        var branch = new Branch
        {
            Name = request.Name,
            Code = request.Code,
            Address = request.Address,
            Phone = request.Phone,
            IsActive = true
        };

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync(cancellationToken);

        return branch.Id;
    }
}
