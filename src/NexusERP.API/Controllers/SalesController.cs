using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Sales.Commands.CreateSale;
using NexusERP.Application.Sales.Queries.GetSales;
using NexusERP.Application.Sales.Queries.GetSaleDetail;
using NexusERP.Application.Sales.CashRegisterSessions.Commands.OpenCashRegisterSession;
using NexusERP.Application.Sales.CashRegisterSessions.Commands.CloseCashRegisterSession;
using NexusERP.Application.Sales.CashRegisterSessions.Queries.GetActiveSession;

namespace NexusERP.API.Controllers;

[Authorize(Roles = "TenantAdmin,TenantUser")]
[ApiController]
[Route("api/v1/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<SaleSummaryDto>>> Get([FromQuery] Guid? branchId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        return await _mediator.Send(new GetSalesQuery { BranchId = branchId, StartDate = startDate, EndDate = endDate });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SaleDetailDto>> GetDetail(Guid id)
    {
        var result = await _mediator.Send(new GetSaleDetailQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateSaleCommand command)
    {
        try
        {
            var saleId = await _mediator.Send(command);
            return Ok(saleId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("sessions/open")]
    public async Task<ActionResult<Guid>> OpenSession(OpenCashRegisterSessionCommand command)
    {
        try
        {
            var sessionId = await _mediator.Send(command);
            return Ok(sessionId);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("sessions/close")]
    public async Task<ActionResult<bool>> CloseSession(CloseCashRegisterSessionCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("sessions/active")]
    public async Task<ActionResult<CashRegisterSessionDto?>> GetActiveSession([FromQuery] Guid cashRegisterId)
    {
        try
        {
            var session = await _mediator.Send(new GetActiveCashRegisterSessionQuery(cashRegisterId));
            if (session == null) return NoContent();
            return Ok(session);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
