using MediatR;

namespace NexusERP.Application.Inventory.Commands.ManualAdjustment;

public class ManualAdjustmentCommand : IRequest<bool>
{
    public Guid ProductVariantId { get; set; }
    public Guid BranchId { get; set; }
    public decimal Quantity { get; set; } // Positive for adding, negative for removing
    public string Reason { get; set; } = string.Empty;
}
