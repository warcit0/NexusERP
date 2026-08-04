using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Platform.Plans.Commands.CreatePlan;
using NexusERP.Application.Platform.Plans.Commands.DeletePlan;
using NexusERP.Application.Platform.Plans.Commands.UpdatePlan;
using NexusERP.Application.Platform.Plans.Queries.GetPlans;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/v1/platform/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class PlansController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<SubscriptionPlanDto>>> GetPlans()
    {
        return Ok(await _mediator.Send(new GetPlansQuery()));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePlanCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdatePlanCommand command)
    {
        if (id != command.Id) return BadRequest("El ID de la ruta no coincide con el del cuerpo.");
        var success = await _mediator.Send(command);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _mediator.Send(new DeletePlanCommand(id));
            return success ? Ok() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
