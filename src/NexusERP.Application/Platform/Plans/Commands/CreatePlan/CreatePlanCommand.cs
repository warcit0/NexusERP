using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities;

namespace NexusERP.Application.Platform.Plans.Commands.CreatePlan;

public record CreatePlanCommand(string Name, string Code, decimal MonthlyPrice, decimal AnnualPrice, int MaxUsers, int MaxBranches, int MaxInvoicesPerMonth, bool IncludesAdvancedAnalytics) : IRequest<Guid>;

public class CreatePlanCommandHandler : IRequestHandler<CreatePlanCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreatePlanCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = new SubscriptionPlan
        {
            Name = request.Name,
            Code = request.Code.ToUpper(),
            MonthlyPrice = request.MonthlyPrice,
            AnnualPrice = request.AnnualPrice,
            MaxUsers = request.MaxUsers,
            MaxBranches = request.MaxBranches,
            MaxInvoicesPerMonth = request.MaxInvoicesPerMonth,
            IncludesAdvancedAnalytics = request.IncludesAdvancedAnalytics
        };

        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync(cancellationToken);

        return plan.Id;
    }
}
