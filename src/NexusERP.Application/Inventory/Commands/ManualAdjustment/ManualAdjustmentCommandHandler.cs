using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Application.Inventory.Commands.ManualAdjustment;

public class ManualAdjustmentCommandHandler : IRequestHandler<ManualAdjustmentCommand, bool>
{
    private readonly INexusDbContext _context;

    public ManualAdjustmentCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ManualAdjustmentCommand request, CancellationToken cancellationToken)
    {
        var balance = await _context.InventoryBalances
            .FirstOrDefaultAsync(b => b.ProductVariantId == request.ProductVariantId && b.BranchId == request.BranchId, cancellationToken);

        if (balance == null)
        {
            balance = new InventoryBalance
            {
                ProductVariantId = request.ProductVariantId,
                BranchId = request.BranchId,
                CurrentStock = 0
            };
            _context.InventoryBalances.Add(balance);
        }

        balance.CurrentStock += request.Quantity;

        var transaction = new InventoryTransaction
        {
            ProductVariantId = request.ProductVariantId,
            BranchId = request.BranchId,
            TransactionType = request.Quantity > 0 ? "AdjustmentIn" : "AdjustmentOut",
            Quantity = Math.Abs(request.Quantity),
            Notes = request.Reason
        };

        _context.InventoryTransactions.Add(transaction);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
