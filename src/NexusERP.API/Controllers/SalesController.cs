using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Sales.Commands.CreateSale;
using NexusERP.Application.Sales.Queries.GetSales;

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
    public async Task<ActionResult<List<SaleSummaryDto>>> Get([FromQuery] Guid? branchId)
    {
        return await _mediator.Send(new GetSalesQuery { BranchId = branchId });
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
}
