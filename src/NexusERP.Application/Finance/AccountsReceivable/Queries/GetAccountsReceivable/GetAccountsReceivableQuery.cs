using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Finance.AccountsReceivable.Queries.GetAccountsReceivable;

public record GetAccountsReceivableQuery : IRequest<List<AccountsReceivableDto>>;

public record AccountsReceivableDto(
    Guid Id,
    Guid CustomerId,
    string CustomerName,
    string InvoiceNumber,
    decimal OriginalAmount,
    decimal BalanceDue,
    DateTime IssueDate,
    DateTime DueDate,
    string Status
);

public class GetAccountsReceivableQueryHandler : IRequestHandler<GetAccountsReceivableQuery, List<AccountsReceivableDto>>
{
    private readonly INexusDbContext _context;

    public GetAccountsReceivableQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountsReceivableDto>> Handle(GetAccountsReceivableQuery request, CancellationToken cancellationToken)
    {
        return await _context.AccountsReceivables
            .Include(x => x.Customer)
            .OrderBy(x => x.DueDate)
            .Select(x => new AccountsReceivableDto(
                x.Id,
                x.CustomerId,
                x.Customer != null ? x.Customer.Name : "",
                x.InvoiceNumber,
                x.OriginalAmount,
                x.BalanceDue,
                x.IssueDate,
                x.DueDate,
                x.Status
            ))
            .ToListAsync(cancellationToken);
    }
}
