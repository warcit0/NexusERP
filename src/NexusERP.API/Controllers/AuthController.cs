using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Auth.Commands.Login;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Microsoft.AspNetCore.RateLimiting.EnableRateLimiting("Auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh([FromBody] NexusERP.Application.Auth.Commands.Refresh.RefreshCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> Logout()
    {
        // Extract token from header
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");
        if (string.IsNullOrEmpty(token)) return BadRequest();

        await _mediator.Send(new NexusERP.Application.Auth.Commands.Logout.LogoutCommand(token));
        return Ok(new { message = "Logged out successfully" });
    }
}
