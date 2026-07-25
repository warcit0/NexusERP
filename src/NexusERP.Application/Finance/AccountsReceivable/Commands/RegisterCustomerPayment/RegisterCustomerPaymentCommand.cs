using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Finance;
using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Application.Finance.AccountsReceivable.Commands.RegisterCustomerPayment;

public record RegisterCustomerPaymentCommand(
    Guid AccountsReceivableId,
    decimal Amount,
    string PaymentMethod,
    string Reference,
    Guid? CashRegisterSessionId,
    string Notes
) : IRequest<Guid>;

public class RegisterCustomerPaymentCommandHandler : IRequestHandler<RegisterCustomerPaymentCommand, Guid>
{
    private readonly INexusDbContext _context;

    public RegisterCustomerPaymentCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegisterCustomerPaymentCommand request, CancellationToken cancellationToken)
    {
        var ar = await _context.AccountsReceivables
            .FirstOrDefaultAsync(x => x.Id == request.AccountsReceivableId, cancellationToken);

        if (ar == null) throw new Exception("La cuenta por cobrar no existe.");
        if (ar.BalanceDue <= 0) throw new Exception("Esta cuenta ya está pagada en su totalidad.");
        if (request.Amount <= 0) throw new Exception("El monto debe ser mayor a 0.");

        // Crear pago
        var payment = new CustomerPayment
        {
            AccountsReceivableId = request.AccountsReceivableId,
            PaymentDate = DateTime.UtcNow,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Reference = request.Reference,
            CashRegisterSessionId = request.CashRegisterSessionId,
            Notes = request.Notes
        };

        _context.CustomerPayments.Add(payment);

        // Actualizar CxC
        ar.BalanceDue -= request.Amount;
        if (ar.BalanceDue <= 0)
        {
            ar.BalanceDue = 0;
            ar.Status = "Paid";
        }
        else
        {
            ar.Status = "PartiallyPaid";
        }

        // Registrar entrada en caja si se especificó
        if (request.CashRegisterSessionId.HasValue)
        {
            var session = await _context.CashRegisterSessions
                .FirstOrDefaultAsync(s => s.Id == request.CashRegisterSessionId.Value, cancellationToken);
                
            if (session != null && session.ClosedAt == null)
            {
                // En un sistema real, aquí podríamos tener una tabla de "CashRegisterTransactions"
                // Pero como actualmente el balance se calcula sumando Ventas, necesitamos guardar este ingreso.
                // Como workaround temporal, podemos dejarlo solo registrado en CustomerPayment 
                // y que el reporte de caja lo sume desde allí.
                // Idealmente deberíamos actualizar session.ActualAmount o tener LogsCaja.
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
