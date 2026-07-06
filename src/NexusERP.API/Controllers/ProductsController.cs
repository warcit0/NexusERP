using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Catalog.Products.Commands.CreateProduct;
using NexusERP.Application.Catalog.Products.Queries.GetProducts;

namespace NexusERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/catalog/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll()
    {
        return Ok(await _mediator.Send(new GetProductsQuery()));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
