using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Settings.CashRegisters.Queries.GetActiveSession;

public class GetActiveSessionQueryHandler : IRequestHandler<GetActiveSessionQuery, ActiveSessionDto?>
{
    private readonly INexusDbContext _context;

    public GetActiveSessionQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<ActiveSessionDto?> Handle(GetActiveSessionQuery request, CancellationToken cancellationToken)
    {
        return await _context.CashRegisterSessions
            .Where(s => s.CashRegisterId == request.CashRegisterId && !s.IsClosed)
            .Include(s => s.CashRegister)
            .Select(s => new ActiveSessionDto
            {
                Id = s.Id,
                CashRegisterId = s.CashRegisterId,
                BranchId = s.CashRegister!.BranchId,
                InitialAmount = s.InitialAmount,
                OpenedAt = s.OpenedAt,
                OpenedByUserId = s.OpenedByUserId
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
