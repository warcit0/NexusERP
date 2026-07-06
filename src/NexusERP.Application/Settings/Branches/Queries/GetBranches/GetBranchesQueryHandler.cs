using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Settings.Branches.Queries.GetBranches;

public class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, List<BranchDto>>
{
    private readonly INexusDbContext _context;

    public GetBranchesQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<BranchDto>> Handle(GetBranchesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Branches
            .OrderBy(b => b.Name)
            .Select(b => new BranchDto(b.Id, b.Name, b.Code, b.Address, b.Phone, b.IsActive))
            .ToListAsync(cancellationToken);
    }
}
