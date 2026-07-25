using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusERP.Application.Reports.Queries.GetSalesReport;

public record GetSalesReportQuery(DateTime StartDate, DateTime EndDate) : IRequest<List<SalesReportItemDto>>;

public record SalesReportItemDto(
    DateTime Date,
    string BranchName,
    int TotalSales,
    decimal TotalRevenue
);

public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, List<SalesReportItemDto>>
{
    private readonly INexusDbContext _context;

    public GetSalesReportQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<SalesReportItemDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        // En SQL, EF Core traducirá esto a GROUP BY
        var sales = await _context.Sales
            .Where(s => s.Date >= request.StartDate && s.Date <= request.EndDate)
            .GroupBy(s => new { s.Date.Date, s.BranchId }) // Asumiendo BranchId, sino general
            .Select(g => new SalesReportItemDto(
                g.Key.Date,
                "General", // Si tuviéramos tabla Branches, la incluiríamos
                g.Count(),
                g.Sum(s => s.Total)
            ))
            .OrderBy(r => r.Date)
            .ToListAsync(cancellationToken);

        return sales;
    }
}
