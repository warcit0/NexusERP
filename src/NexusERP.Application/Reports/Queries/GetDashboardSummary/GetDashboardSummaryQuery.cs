using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Reports.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;

public class DashboardSummaryDto
{
    public decimal SalesToday { get; set; }
    public int TransactionsToday { get; set; }
    public decimal TotalReceivablesPending { get; set; }
    public int OverdueReceivables { get; set; }
    public decimal TotalPayablesPending { get; set; }
    public int OverduePayables { get; set; }
    public int CriticalStockItems { get; set; }
    public List<DailySaleDto> Last7DaysSales { get; set; } = new();
}

public class DailySaleDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int Count { get; set; }
}

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly INexusDbContext _context;

    public GetDashboardSummaryQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var todayEnd = todayStart.AddDays(1);
        var sevenDaysAgo = todayStart.AddDays(-6);

        // Ventas de hoy
        var todaySales = await _context.Sales
            .Where(s => s.Date >= todayStart && s.Date < todayEnd)
            .Select(s => new { s.Total })
            .ToListAsync(cancellationToken);

        // Ventas de los últimos 7 días agrupadas por día
        var last7DaysSales = await _context.Sales
            .Where(s => s.Date >= sevenDaysAgo && s.Date < todayEnd)
            .GroupBy(s => s.Date.Date)
            .Select(g => new
            {
                Date = g.Key,
                Total = g.Sum(s => s.Total),
                Count = g.Count()
            })
            .OrderBy(g => g.Date)
            .ToListAsync(cancellationToken);

        // CxC pendientes
        var receivables = await _context.AccountsReceivables
            .Where(r => r.Status != "Paid")
            .Select(r => new { r.BalanceDue, r.DueDate })
            .ToListAsync(cancellationToken);

        // CxP pendientes
        var payables = await _context.AccountsPayables
            .Where(p => p.Status != "Paid")
            .Select(p => new { p.BalanceDue, p.DueDate })
            .ToListAsync(cancellationToken);

        // Stock crítico (< 5 unidades)
        var criticalStock = await _context.InventoryBalances
            .Where(b => b.CurrentStock < 5)
            .CountAsync(cancellationToken);

        // Construir los últimos 7 días (rellenando días sin ventas con 0)
        var dailySales = new List<DailySaleDto>();
        for (int i = 6; i >= 0; i--)
        {
            var day = todayStart.AddDays(-i);
            var dayData = last7DaysSales.FirstOrDefault(d => d.Date == day);
            dailySales.Add(new DailySaleDto
            {
                Label = day.ToString("ddd dd/MM"),
                Total = dayData?.Total ?? 0,
                Count = dayData?.Count ?? 0
            });
        }

        return new DashboardSummaryDto
        {
            SalesToday = todaySales.Sum(s => s.Total),
            TransactionsToday = todaySales.Count,
            TotalReceivablesPending = receivables.Sum(r => r.BalanceDue),
            OverdueReceivables = receivables.Count(r => r.DueDate < now),
            TotalPayablesPending = payables.Sum(p => p.BalanceDue),
            OverduePayables = payables.Count(p => p.DueDate < now),
            CriticalStockItems = criticalStock,
            Last7DaysSales = dailySales
        };
    }
}
