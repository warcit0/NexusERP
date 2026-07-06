using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Application.Catalog.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateBrandCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var brand = new Brand
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };

        _context.Brands.Add(brand);
        await _context.SaveChangesAsync(cancellationToken);

        return brand.Id;
    }
}
