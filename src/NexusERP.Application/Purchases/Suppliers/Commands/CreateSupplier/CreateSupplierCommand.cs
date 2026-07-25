using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Purchases;

namespace NexusERP.Application.Purchases.Suppliers.Commands.CreateSupplier;

public record CreateSupplierCommand(string Name, string TaxId, string Email, string Phone, string Address) : IRequest<Guid>;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateSupplierCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var supplier = new Supplier
        {
            Name = request.Name,
            TaxId = request.TaxId,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return supplier.Id;
    }
}
