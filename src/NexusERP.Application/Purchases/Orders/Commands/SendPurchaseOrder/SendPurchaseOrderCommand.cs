using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Purchases.Orders.Commands.SendPurchaseOrder;

public record SendPurchaseOrderCommand(Guid PurchaseOrderId) : IRequest<bool>;

public class SendPurchaseOrderCommandHandler : IRequestHandler<SendPurchaseOrderCommand, bool>
{
    private readonly INexusDbContext _context;

    public SendPurchaseOrderCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SendPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(o => o.Id == request.PurchaseOrderId, cancellationToken);

        if (order == null)
            throw new Exception("Orden de compra no encontrada.");

        if (order.Status != "Draft")
            throw new Exception($"Solo se pueden enviar órdenes en estado 'Draft'. Estado actual: {order.Status}.");

        order.Status = "Sent";

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
