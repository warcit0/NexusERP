using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Purchases.Suppliers.Commands.CreateSupplier;
using NexusERP.Application.Purchases.Suppliers.Commands.UpdateSupplier;
using NexusERP.Application.Purchases.Suppliers.Queries.GetSuppliers;
using NexusERP.Application.Purchases.Orders.Commands.CreatePurchaseOrder;
using NexusERP.Application.Purchases.Orders.Commands.SendPurchaseOrder;
using NexusERP.Application.Purchases.Orders.Queries.GetPurchaseOrders;
using NexusERP.Application.Purchases.Orders.Queries.GetPurchaseOrderById;
using NexusERP.Application.Purchases.Orders.Commands.ReceivePurchaseOrder;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Asume que la auth base ya está configurada
public class PurchasesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PurchasesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("suppliers")]
    public async Task<ActionResult<List<SupplierDto>>> GetSuppliers()
    {
        return Ok(await _mediator.Send(new GetSuppliersQuery()));
    }

    [HttpPost("suppliers")]
    public async Task<ActionResult<Guid>> CreateSupplier(CreateSupplierCommand command)
    {
        var supplierId = await _mediator.Send(command);
        return Ok(supplierId);
    }

    [HttpPut("suppliers/{id}")]
    public async Task<ActionResult> UpdateSupplier(Guid id, UpdateSupplierCommand command)
    {
        if (id != command.Id) return BadRequest("El ID de la ruta no coincide con el comando.");
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("orders")]
    public async Task<ActionResult<List<PurchaseOrderDto>>> GetPurchaseOrders()
    {
        return Ok(await _mediator.Send(new GetPurchaseOrdersQuery()));
    }

    [HttpGet("orders/{id}")]
    public async Task<ActionResult<PurchaseOrderDetailDto>> GetPurchaseOrderById(Guid id)
    {
        var result = await _mediator.Send(new GetPurchaseOrderByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("orders")]
    public async Task<ActionResult<Guid>> CreatePurchaseOrder(CreatePurchaseOrderCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPost("orders/{id}/receive")]
    public async Task<ActionResult<bool>> ReceivePurchaseOrder(Guid id, ReceivePurchaseOrderCommand command)
    {
        if (id != command.PurchaseOrderId)
            return BadRequest("El ID de la ruta no coincide con el comando.");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("orders/{id}/send")]
    public async Task<ActionResult<bool>> SendPurchaseOrder(Guid id)
    {
        try
        {
            var result = await _mediator.Send(new SendPurchaseOrderCommand(id));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
