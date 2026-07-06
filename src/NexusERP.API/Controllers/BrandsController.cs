using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Catalog.Brands.Commands.CreateBrand;
using NexusERP.Application.Catalog.Brands.Queries.GetBrands;

namespace NexusERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/catalog/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<BrandDto>>> GetAll()
    {
        return Ok(await _mediator.Send(new GetBrandsQuery()));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateBrandCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
