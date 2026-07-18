using MediatR;

namespace NexusERP.Application.Sales.Customers.Queries.GetCustomers;

public class GetCustomersQuery : IRequest<List<CustomerDto>>
{
}

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
