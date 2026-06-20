using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Identity.Commands.CreateUser;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo usuario dentro del Tenant del administrador, o como SuperAdmin.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<CreateUserResponse>> Create([FromBody] CreateUserCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
