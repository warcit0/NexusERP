using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Platform.Plans.Queries.GetPlans;

public record SubscriptionPlanDto(Guid Id, string Name, string Code, decimal MonthlyPrice, decimal AnnualPrice, int MaxUsers, int MaxBranches, int MaxInvoicesPerMonth, bool IncludesAdvancedAnalytics);

public record GetPlansQuery() : IRequest<List<SubscriptionPlanDto>>;

public class GetPlansQueryHandler : IRequestHandler<GetPlansQuery, List<SubscriptionPlanDto>>
{
    private readonly INexusDbContext _context;

    public GetPlansQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubscriptionPlanDto>> Handle(GetPlansQuery request, CancellationToken cancellationToken)
    {
        return await _context.SubscriptionPlans
            .Select(p => new SubscriptionPlanDto(p.Id, p.Name, p.Code, p.MonthlyPrice, p.AnnualPrice, p.MaxUsers, p.MaxBranches, p.MaxInvoicesPerMonth, p.IncludesAdvancedAnalytics))
            .ToListAsync(cancellationToken);
    }
}
