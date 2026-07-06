using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Settings.CashRegisters.Queries.GetCashRegisters;

public class GetCashRegistersQueryHandler : IRequestHandler<GetCashRegistersQuery, List<CashRegisterDto>>
{
    private readonly INexusDbContext _context;

    public GetCashRegistersQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<CashRegisterDto>> Handle(GetCashRegistersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.CashRegisters
            .Include(c => c.Branch)
            .AsQueryable();

        if (request.BranchId.HasValue)
        {
            query = query.Where(c => c.BranchId == request.BranchId.Value);
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new CashRegisterDto(
                c.Id, 
                c.BranchId, 
                c.Branch!.Name, 
                c.Name, 
                c.MacAddress, 
                c.IsActive, 
                c.IsOpen))
            .ToListAsync(cancellationToken);
    }
}
