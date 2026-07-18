using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Settings.CashRegisters.Commands.CloseSession;

public class CloseSessionCommandHandler : IRequestHandler<CloseSessionCommand, bool>
{
    private readonly INexusDbContext _context;

    public CloseSessionCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CloseSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.CashRegisterSessions
            .Include(s => s.CashRegister)
            .FirstOrDefaultAsync(s => s.CashRegisterId == request.CashRegisterId && !s.IsClosed, cancellationToken);

        if (session == null)
            throw new Exception("No hay una sesión abierta para esta caja.");

        var userId = "system";

        session.ClosedByUserId = userId;
        session.ClosedAt = DateTime.UtcNow;
        session.FinalAmount = request.FinalAmount;
        session.Notes = string.IsNullOrEmpty(request.Notes) ? session.Notes : request.Notes;
        session.IsClosed = true;
        
        if (session.CashRegister != null)
            session.CashRegister.IsOpen = false; // Marcar caja como cerrada

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
