using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Finance.AccountsPayable.Queries.GetAccountPayments;

public record GetSupplierPaymentsQuery(Guid AccountsPayableId) : IRequest<List<SupplierPaymentDto>>;

public class SupplierPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class GetSupplierPaymentsQueryHandler : IRequestHandler<GetSupplierPaymentsQuery, List<SupplierPaymentDto>>
{
    private readonly INexusDbContext _context;

    public GetSupplierPaymentsQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplierPaymentDto>> Handle(GetSupplierPaymentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.SupplierPayments
            .Where(p => p.AccountsPayableId == request.AccountsPayableId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new SupplierPaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                Reference = p.Reference,
                Notes = p.Notes
            })
            .ToListAsync(cancellationToken);
    }
}
