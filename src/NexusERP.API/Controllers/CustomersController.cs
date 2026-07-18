using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Sales.Customers.Commands.CreateCustomer;
using NexusERP.Application.Sales.Customers.Queries.GetCustomers;

namespace NexusERP.API.Controllers;

[Authorize(Roles = "TenantAdmin,TenantUser")]
[ApiController]
[Route("api/v1/sales/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerDto>>> Get()
    {
        return await _mediator.Send(new GetCustomersQuery());
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCustomerCommand command)
    {
        return await _mediator.Send(command);
    }
}
