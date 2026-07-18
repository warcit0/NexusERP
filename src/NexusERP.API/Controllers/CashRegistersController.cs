using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Settings.CashRegisters.Commands.CreateCashRegister;
using NexusERP.Application.Settings.CashRegisters.Commands.OpenSession;
using NexusERP.Application.Settings.CashRegisters.Commands.CloseSession;
using NexusERP.Application.Settings.CashRegisters.Queries.GetCashRegisters;
using NexusERP.Application.Settings.CashRegisters.Queries.GetActiveSession;

namespace NexusERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/settings/[controller]")]
public class CashRegistersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CashRegistersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CashRegisterDto>>> GetAll([FromQuery] Guid? branchId)
    {
        return Ok(await _mediator.Send(new GetCashRegistersQuery(branchId)));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCashRegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpGet("{cashRegisterId}/session")]
    public async Task<ActionResult<ActiveSessionDto?>> GetActiveSession(Guid cashRegisterId)
    {
        var session = await _mediator.Send(new GetActiveSessionQuery { CashRegisterId = cashRegisterId });
        return Ok(session);
    }

    [HttpPost("{cashRegisterId}/open")]
    public async Task<ActionResult<Guid>> OpenSession(Guid cashRegisterId, [FromBody] OpenSessionRequest request)
    {
        try
        {
            var id = await _mediator.Send(new OpenSessionCommand
            {
                CashRegisterId = cashRegisterId,
                InitialAmount = request.InitialAmount,
                Notes = request.Notes
            });
            return Ok(id);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("{cashRegisterId}/close")]
    public async Task<ActionResult> CloseSession(Guid cashRegisterId, [FromBody] CloseSessionRequest request)
    {
        try
        {
            await _mediator.Send(new CloseSessionCommand
            {
                CashRegisterId = cashRegisterId,
                FinalAmount = request.FinalAmount,
                Notes = request.Notes
            });
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

public record OpenSessionRequest(decimal InitialAmount, string Notes = "");
public record CloseSessionRequest(decimal FinalAmount, string Notes = "");
