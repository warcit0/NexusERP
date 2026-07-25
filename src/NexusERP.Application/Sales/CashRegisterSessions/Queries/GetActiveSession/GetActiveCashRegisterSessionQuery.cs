using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Sales.CashRegisterSessions.Queries.GetActiveSession;

public record GetActiveCashRegisterSessionQuery(Guid CashRegisterId) : IRequest<CashRegisterSessionDto?>;

public record CashRegisterSessionDto(Guid Id, Guid CashRegisterId, Guid BranchId, decimal InitialAmount, DateTime OpenedAt, string OpenedByUserId);

public class GetActiveCashRegisterSessionQueryHandler : IRequestHandler<GetActiveCashRegisterSessionQuery, CashRegisterSessionDto?>
{
    private readonly INexusDbContext _context;

    public GetActiveCashRegisterSessionQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<CashRegisterSessionDto?> Handle(GetActiveCashRegisterSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .Where(s => s.CashRegisterId == request.CashRegisterId && !s.IsClosed)
            .Select(s => new CashRegisterSessionDto(s.Id, s.CashRegisterId, s.CashRegister!.BranchId, s.InitialAmount, s.OpenedAt, s.OpenedByUserId))
            .FirstOrDefaultAsync(cancellationToken);
            
        return session;
    }
}
