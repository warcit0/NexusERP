using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Identity.Commands.CreateUser;
using NexusERP.Application.Identity.Queries.GetUsers;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "SuperAdmin,TenantAdmin")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IIdentityService _identityService;

    public UsersController(IMediator mediator, IIdentityService identityService)
    {
        _mediator = mediator;
        _identityService = identityService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Ok(users);
    }

    /// <summary>
    /// Crea un nuevo usuario dentro del Tenant del administrador, o como SuperAdmin.
    /// </summary>
    [HttpPost]
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

    [HttpPatch("{id}/active")]
    public async Task<ActionResult> SetActive(string id, [FromBody] SetActiveRequest request)
    {
        var result = await _identityService.SetUserActiveAsync(id, request.IsActive);
        if (!result) return NotFound();
        return NoContent();
    }
}

public class SetActiveRequest
{
    public bool IsActive { get; set; }
}
