using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Settings.CashRegisters.Commands.CreateCashRegister;
using NexusERP.Application.Settings.CashRegisters.Queries.GetCashRegisters;

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
}
