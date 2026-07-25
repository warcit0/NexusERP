using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Finance.AccountsPayable.Queries.GetAccountsPayable;

public record GetAccountsPayableQuery : IRequest<List<AccountsPayableDto>>;

public record AccountsPayableDto(
    Guid Id,
    Guid SupplierId,
    string SupplierName,
    string InvoiceNumber,
    decimal OriginalAmount,
    decimal BalanceDue,
    DateTime IssueDate,
    DateTime DueDate,
    string Status
);

public class GetAccountsPayableQueryHandler : IRequestHandler<GetAccountsPayableQuery, List<AccountsPayableDto>>
{
    private readonly INexusDbContext _context;

    public GetAccountsPayableQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<AccountsPayableDto>> Handle(GetAccountsPayableQuery request, CancellationToken cancellationToken)
    {
        return await _context.AccountsPayables
            .Include(x => x.Supplier)
            .OrderBy(x => x.DueDate)
            .Select(x => new AccountsPayableDto(
                x.Id,
                x.SupplierId,
                x.Supplier != null ? x.Supplier.Name : "",
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
