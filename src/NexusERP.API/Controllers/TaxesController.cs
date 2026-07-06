using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Catalog.Taxes.Commands.CreateTax;
using NexusERP.Application.Catalog.Taxes.Queries.GetTaxes;

namespace NexusERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/catalog/[controller]")]
public class TaxesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaxesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<TaxDto>>> GetAll()
    {
        return Ok(await _mediator.Send(new GetTaxesQuery()));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateTaxCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
