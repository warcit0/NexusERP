using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Finance;

namespace NexusERP.Application.Finance.AccountsPayable.Commands.RegisterSupplierPayment;

public record RegisterSupplierPaymentCommand(
    Guid AccountsPayableId,
    decimal Amount,
    string PaymentMethod,
    string Reference,
    Guid? CashRegisterSessionId,
    string Notes
) : IRequest<Guid>;

public class RegisterSupplierPaymentCommandHandler : IRequestHandler<RegisterSupplierPaymentCommand, Guid>
{
    private readonly INexusDbContext _context;

    public RegisterSupplierPaymentCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RegisterSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var ap = await _context.AccountsPayables
            .FirstOrDefaultAsync(x => x.Id == request.AccountsPayableId, cancellationToken);

        if (ap == null) throw new Exception("La cuenta por pagar no existe.");
        if (ap.BalanceDue <= 0) throw new Exception("Esta cuenta ya está pagada en su totalidad.");
        if (request.Amount <= 0) throw new Exception("El monto debe ser mayor a 0.");

        // Crear pago
        var payment = new SupplierPayment
        {
            AccountsPayableId = request.AccountsPayableId,
            PaymentDate = DateTime.UtcNow,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Reference = request.Reference,
            CashRegisterSessionId = request.CashRegisterSessionId,
            Notes = request.Notes
        };

        _context.SupplierPayments.Add(payment);

        // Actualizar CxP
        ap.BalanceDue -= request.Amount;
        if (ap.BalanceDue <= 0)
        {
            ap.BalanceDue = 0;
            ap.Status = "Paid";
        }
        else
        {
            ap.Status = "PartiallyPaid";
        }

        await _context.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
