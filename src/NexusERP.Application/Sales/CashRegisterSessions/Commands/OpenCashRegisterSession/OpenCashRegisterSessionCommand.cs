using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Application.Sales.CashRegisterSessions.Commands.OpenCashRegisterSession;

public record OpenCashRegisterSessionCommand(Guid CashRegisterId, decimal InitialAmount) : IRequest<Guid>;

public class OpenCashRegisterSessionCommandHandler : IRequestHandler<OpenCashRegisterSessionCommand, Guid>
{
    private readonly INexusDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public OpenCashRegisterSessionCommandHandler(INexusDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> Handle(OpenCashRegisterSessionCommand request, CancellationToken cancellationToken)
    {
        // Verificar si ya hay una sesión abierta para esta caja
        var existingSession = await _context.CashRegisterSessions
            .FirstOrDefaultAsync(s => s.CashRegisterId == request.CashRegisterId && !s.IsClosed, cancellationToken);
            
        if (existingSession != null)
            throw new Exception("Ya existe una sesión de caja abierta para esta caja registradora.");

        var session = new CashRegisterSession
        {
            CashRegisterId = request.CashRegisterId,
            InitialAmount = request.InitialAmount,
            OpenedAt = DateTime.UtcNow,
            OpenedByUserId = _currentUserService.UserId ?? "System",
            IsClosed = false
        };

        _context.CashRegisterSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}
