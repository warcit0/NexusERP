using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Platform.Plans.Commands.UpdatePlan;

public record UpdatePlanCommand(Guid Id, string Name, string Code, decimal MonthlyPrice, decimal AnnualPrice, int MaxUsers, int MaxBranches, int MaxInvoicesPerMonth, bool IncludesAdvancedAnalytics) : IRequest<bool>;

public class UpdatePlanCommandHandler : IRequestHandler<UpdatePlanCommand, bool>
{
    private readonly INexusDbContext _context;

    public UpdatePlanCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdatePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (plan == null) return false;

        plan.Name = request.Name;
        plan.Code = request.Code.ToUpper();
        plan.MonthlyPrice = request.MonthlyPrice;
        plan.AnnualPrice = request.AnnualPrice;
        plan.MaxUsers = request.MaxUsers;
        plan.MaxBranches = request.MaxBranches;
        plan.MaxInvoicesPerMonth = request.MaxInvoicesPerMonth;
        plan.IncludesAdvancedAnalytics = request.IncludesAdvancedAnalytics;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
