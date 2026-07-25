using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Reports.Queries.GetReceivablesAging;

public record GetReceivablesAgingQuery : IRequest<ReceivablesAgingDto>;

public class ReceivablesAgingDto
{
    public List<AgingBucketDto> Buckets { get; set; } = new();
    public decimal TotalPending { get; set; }
}

public class AgingBucketDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

public class ReceivablesAgingItem
{
    public string CustomerName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal BalanceDue { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public string Bucket { get; set; } = string.Empty;
}

public class GetReceivablesAgingQueryHandler : IRequestHandler<GetReceivablesAgingQuery, ReceivablesAgingDto>
{
    private readonly INexusDbContext _context;

    public GetReceivablesAgingQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<ReceivablesAgingDto> Handle(GetReceivablesAgingQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var pending = await _context.AccountsReceivables
            .Where(r => r.Status != "Paid" && r.BalanceDue > 0)
            .Include(r => r.Customer)
            .Select(r => new
            {
                r.BalanceDue,
                r.DueDate,
                DaysOverdue = (today - r.DueDate.Date).Days
            })
            .ToListAsync(cancellationToken);

        var buckets = new[]
        {
            new AgingBucketDto { Label = "Al día (no vencido)", Count = 0, Amount = 0 },
            new AgingBucketDto { Label = "1 – 30 días",         Count = 0, Amount = 0 },
            new AgingBucketDto { Label = "31 – 60 días",        Count = 0, Amount = 0 },
            new AgingBucketDto { Label = "61 – 90 días",        Count = 0, Amount = 0 },
            new AgingBucketDto { Label = "+ 90 días",           Count = 0, Amount = 0 },
        };

        foreach (var item in pending)
        {
            var idx = item.DaysOverdue switch
            {
                <= 0      => 0,
                <= 30     => 1,
                <= 60     => 2,
                <= 90     => 3,
                _         => 4
            };
            buckets[idx].Count++;
            buckets[idx].Amount += item.BalanceDue;
        }

        return new ReceivablesAgingDto
        {
            Buckets = buckets.ToList(),
            TotalPending = pending.Sum(x => x.BalanceDue)
        };
    }
}
