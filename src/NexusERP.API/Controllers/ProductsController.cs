using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Catalog.Products.Commands.CreateProduct;
using NexusERP.Application.Catalog.Products.Commands.UpdateProductVariant;
using NexusERP.Application.Catalog.Products.Queries.GetProducts;
using NexusERP.Application.Catalog.Products.Queries.GetProductVariants;

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

    [HttpGet("variants")]
    public async Task<ActionResult<List<FlatProductVariantDto>>> GetVariants()
    {
        return Ok(await _mediator.Send(new GetProductVariantsQuery()));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    [HttpPut("variants/{variantId}")]
    public async Task<ActionResult> UpdateVariant(Guid variantId, UpdateProductVariantCommand command)
    {
        command.VariantId = variantId;
        await _mediator.Send(command);
        return Ok();
    }
}
