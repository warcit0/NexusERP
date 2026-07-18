using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Inventory.Queries.GetInventoryBalances;
using NexusERP.Application.Inventory.Commands.ManualAdjustment;

namespace NexusERP.API.Controllers;

[Authorize(Roles = "TenantAdmin,TenantUser")]
[ApiController]
[Route("api/v1/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("balances")]
    public async Task<ActionResult<List<InventoryBalanceDto>>> GetBalances([FromQuery] Guid? branchId)
    {
        return await _mediator.Send(new GetInventoryBalancesQuery { BranchId = branchId });
    }

    [HttpPost("adjust")]
    public async Task<ActionResult> AdjustInventory(ManualAdjustmentCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}
