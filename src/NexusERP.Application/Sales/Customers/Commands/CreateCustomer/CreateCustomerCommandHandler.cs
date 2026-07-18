using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Application.Sales.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateCustomerCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Identification = request.Identification,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
