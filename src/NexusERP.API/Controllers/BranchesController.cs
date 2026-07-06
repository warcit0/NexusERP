using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Settings.Branches.Commands.CreateBranch;
using NexusERP.Application.Settings.Branches.Queries.GetBranches;

namespace NexusERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/settings/[controller]")]
public class BranchesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BranchesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<BranchDto>>> GetAll()
    {
        return Ok(await _mediator.Send(new GetBranchesQuery()));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateBranchCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
