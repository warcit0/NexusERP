using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Platform.Tenants.Commands.CreateTenant;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/v1/platform/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crea un nuevo Tenant con su usuario administrador.
    /// Solo accesible para SuperAdmins de la plataforma.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<CreateTenantResponse>> Create([FromBody] CreateTenantCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = response.TenantId }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un Tenant por ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        // TODO: Implementar GetTenantQuery
        return Ok(new { id });
    }
}
