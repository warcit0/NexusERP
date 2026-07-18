using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Application.Settings.CashRegisters.Commands.OpenSession;

public class OpenSessionCommandHandler : IRequestHandler<OpenSessionCommand, Guid>
{
    private readonly INexusDbContext _context;

    public OpenSessionCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(OpenSessionCommand request, CancellationToken cancellationToken)
    {
        // Verificar que la caja existe
        var cashRegister = await _context.CashRegisters
            .FirstOrDefaultAsync(cr => cr.Id == request.CashRegisterId, cancellationToken);

        if (cashRegister == null)
            throw new Exception("La caja registradora no existe.");

        // Verificar que no haya una sesión activa ya abierta para esta caja
        var existingSession = await _context.CashRegisterSessions
            .FirstOrDefaultAsync(s => s.CashRegisterId == request.CashRegisterId && !s.IsClosed, cancellationToken);

        if (existingSession != null)
            throw new Exception("Esta caja ya tiene una sesión abierta.");

        var userId = "system";

        var session = new CashRegisterSession
        {
            CashRegisterId = request.CashRegisterId,
            OpenedByUserId = userId,
            OpenedAt = DateTime.UtcNow,
            InitialAmount = request.InitialAmount,
            Notes = request.Notes,
            IsClosed = false
        };

        _context.CashRegisterSessions.Add(session);
        
        cashRegister.IsOpen = true; // Marcar la caja como abierta
        
        await _context.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}
