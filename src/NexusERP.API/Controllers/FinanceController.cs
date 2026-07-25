using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Finance.AccountsReceivable.Queries.GetAccountsReceivable;
using NexusERP.Application.Finance.AccountsReceivable.Commands.RegisterCustomerPayment;
using NexusERP.Application.Finance.AccountsPayable.Queries.GetAccountsPayable;
using NexusERP.Application.Finance.AccountsPayable.Commands.RegisterSupplierPayment;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --- Cuentas por Cobrar ---
    [HttpGet("receivables")]
    public async Task<ActionResult<List<AccountsReceivableDto>>> GetAccountsReceivable()
    {
        return Ok(await _mediator.Send(new GetAccountsReceivableQuery()));
    }

    [HttpPost("receivables/{id}/pay")]
    public async Task<ActionResult<Guid>> RegisterCustomerPayment(Guid id, RegisterCustomerPaymentCommand command)
    {
        if (id != command.AccountsReceivableId)
            return BadRequest("El ID de la ruta no coincide con el comando.");

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // --- Cuentas por Pagar ---
    [HttpGet("payables")]
    public async Task<ActionResult<List<AccountsPayableDto>>> GetAccountsPayable()
    {
        return Ok(await _mediator.Send(new GetAccountsPayableQuery()));
    }

    [HttpPost("payables/{id}/pay")]
    public async Task<ActionResult<Guid>> RegisterSupplierPayment(Guid id, RegisterSupplierPaymentCommand command)
    {
        if (id != command.AccountsPayableId)
            return BadRequest("El ID de la ruta no coincide con el comando.");

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
