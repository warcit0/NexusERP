using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Finance.AccountsReceivable.Queries.GetAccountPayments;

public record GetCustomerPaymentsQuery(Guid AccountsReceivableId) : IRequest<List<CustomerPaymentDto>>;

public class CustomerPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class GetCustomerPaymentsQueryHandler : IRequestHandler<GetCustomerPaymentsQuery, List<CustomerPaymentDto>>
{
    private readonly INexusDbContext _context;

    public GetCustomerPaymentsQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerPaymentDto>> Handle(GetCustomerPaymentsQuery request, CancellationToken cancellationToken)
    {
        return await _context.CustomerPayments
            .Where(p => p.AccountsReceivableId == request.AccountsReceivableId)
            .OrderByDescending(p => p.PaymentDate)
            .Select(p => new CustomerPaymentDto
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
