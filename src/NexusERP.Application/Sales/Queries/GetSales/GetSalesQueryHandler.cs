using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Sales.Queries.GetSales;

public class GetSalesQueryHandler : IRequestHandler<GetSalesQuery, List<SaleSummaryDto>>
{
    private readonly INexusDbContext _context;

    public GetSalesQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<SaleSummaryDto>> Handle(GetSalesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Sales
            .Include(s => s.Customer)
            .AsNoTracking();

        if (request.BranchId.HasValue)
            query = query.Where(s => s.BranchId == request.BranchId.Value);

        if (request.StartDate.HasValue)
            query = query.Where(s => s.Date >= request.StartDate.Value.Date);

        if (request.EndDate.HasValue)
            query = query.Where(s => s.Date < request.EndDate.Value.Date.AddDays(1));

        return await query
            .OrderByDescending(s => s.Date)
            .Select(s => new SaleSummaryDto
            {
                Id = s.Id,
                ReceiptNumber = s.ReceiptNumber,
                Date = s.Date,
                CustomerName = s.Customer != null ? s.Customer.Name : "Consumidor Final",
                Total = s.Total,
                Status = s.Status
            })
            .ToListAsync(cancellationToken);
    }
}
