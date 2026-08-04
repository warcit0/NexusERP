using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Platform.Licenses.Commands.ActivateLicense;
using NexusERP.Application.Platform.Licenses.Commands.AssignLicense;
using NexusERP.Application.Platform.Licenses.Queries.GetLicenses;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/v1/platform/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class LicensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LicensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<LicenseDto>>> GetLicenses()
    {
        return Ok(await _mediator.Send(new GetLicensesQuery()));
    }

    [HttpPost("assign")]
    public async Task<ActionResult> AssignLicense([FromBody] AssignLicenseCommand command)
    {
        var success = await _mediator.Send(command);
        return success ? Ok() : BadRequest("Error al asignar licencia. Verifique el Tenant y Plan.");
    }

    /// <summary>
    /// Activa una licencia para un Tenant. La firma HMAC-SHA256 garantiza que el código
    /// no ha sido manipulado. Solo accesible para SuperAdmins.
    /// </summary>
    [HttpPost("activate")]
    public async Task<ActionResult<ActivateLicenseResponse>> Activate([FromBody] ActivateLicenseCommand command)
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
