using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Sales.CashRegisterSessions.Commands.CloseCashRegisterSession;

public record CloseCashRegisterSessionCommand(Guid SessionId, decimal ActualAmount, string Notes) : IRequest<bool>;

public class CloseCashRegisterSessionCommandHandler : IRequestHandler<CloseCashRegisterSessionCommand, bool>
{
    private readonly INexusDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CloseCashRegisterSessionCommandHandler(INexusDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(CloseCashRegisterSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.CashRegisterSessions
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);
            
        if (session == null)
            throw new Exception("La sesión de caja no existe.");
            
        if (session.IsClosed)
            throw new Exception("La sesión de caja ya se encuentra cerrada.");

        // Calcular Expected Amount: Initial + Pagos en Efectivo
        var salesInCash = await _context.Sales
            .Include(s => s.Payments)
            .Where(s => s.CashRegisterSessionId == request.SessionId)
            .SelectMany(s => s.Payments)
            .Where(p => p.PaymentMethod == "Efectivo")
            .SumAsync(p => p.Amount, cancellationToken);
            
        // Podrían haber ingresos/egresos adicionales (Caja Chica, retiros, etc.), pero por ahora solo Ventas.
        
        session.ExpectedAmount = session.InitialAmount + salesInCash;
        session.FinalAmount = request.ActualAmount;
        session.Difference = request.ActualAmount - session.ExpectedAmount;
        
        session.ClosedAt = DateTime.UtcNow;
        session.ClosedByUserId = _currentUserService.UserId ?? "System";
        session.IsClosed = true;
        session.FinalAmount = request.ActualAmount;
        session.Notes = request.Notes;
        
        var register = await _context.CashRegisters.FindAsync(session.CashRegisterId);
        if (register != null)
        {
            register.IsOpen = false;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
