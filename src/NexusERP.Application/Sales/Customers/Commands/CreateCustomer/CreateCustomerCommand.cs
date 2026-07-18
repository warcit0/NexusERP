using MediatR;

namespace NexusERP.Application.Sales.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
