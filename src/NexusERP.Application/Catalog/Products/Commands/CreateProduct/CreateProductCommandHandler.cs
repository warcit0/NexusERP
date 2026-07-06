using MediatR;
using NexusERP.Application.Common.Interfaces;
using NexusERP.Domain.Entities.Catalog;

namespace NexusERP.Application.Catalog.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly INexusDbContext _context;

    public CreateProductCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            TaxId = request.TaxId,
            CabysCode = request.CabysCode,
            MeasurementUnit = request.MeasurementUnit,
            IsActive = true
        };

        foreach (var v in request.Variants)
        {
            product.Variants.Add(new ProductVariant
            {
                Sku = v.Sku,
                Barcode = v.Barcode,
                Size = v.Size,
                Color = v.Color,
                Cost = v.Cost,
                Price = v.Price,
                IsActive = true
            });
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
