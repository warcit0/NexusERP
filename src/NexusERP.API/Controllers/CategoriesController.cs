using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Catalog.Categories.Commands.CreateCategory;
using NexusERP.Application.Catalog.Categories.Queries.GetCategories;

namespace NexusERP.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/catalog/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        return Ok(await _mediator.Send(new GetCategoriesQuery()));
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCategoryCommand command)
    {
        return Ok(await _mediator.Send(command));
    }
}
