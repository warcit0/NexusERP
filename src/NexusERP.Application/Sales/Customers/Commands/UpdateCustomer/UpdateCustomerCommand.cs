using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Sales.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal CreditLimit { get; set; }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, bool>
{
    private readonly INexusDbContext _context;

    public UpdateCustomerCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity == null)
            throw new Exception("Cliente no encontrado.");

        entity.Name = request.Name;
        entity.Identification = request.Identification;
        entity.Email = request.Email;
        entity.Phone = request.Phone;
        entity.Address = request.Address;
        entity.CreditLimit = request.CreditLimit;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
