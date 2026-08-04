using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Platform.Plans.Commands.DeletePlan;

public record DeletePlanCommand(Guid Id) : IRequest<bool>;

public class DeletePlanCommandHandler : IRequestHandler<DeletePlanCommand, bool>
{
    private readonly INexusDbContext _context;

    public DeletePlanCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _context.SubscriptionPlans
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            
        if (plan == null) return false;
        
        if (plan.Tenants.Any())
            throw new InvalidOperationException("No se puede eliminar un plan que tiene inquilinos asignados.");

        _context.SubscriptionPlans.Remove(plan);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
