using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Application.Catalog.Taxes.Commands.CreateTax;

public class CreateTaxCommandHandler : IRequestHandler<CreateTaxCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateTaxCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateTaxCommand request, CancellationToken cancellationToken)
    {
        var tax = new Tax
        {
            Name = request.Name,
            Percentage = request.Percentage,
            IsActive = true
        };

        _context.Taxes.Add(tax);
        await _context.SaveChangesAsync(cancellationToken);

        return tax.Id;
    }
}
