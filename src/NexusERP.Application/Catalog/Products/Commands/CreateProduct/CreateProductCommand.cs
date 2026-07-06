using MediatR;

namespace NexusERP.Application.Catalog.Products.Commands.CreateProduct;

public record CreateProductVariantDto(string Sku, string Barcode, string Size, string Color, decimal Cost, decimal Price);

public record CreateProductCommand(
    string Name, 
    string Description, 
    Guid CategoryId, 
    Guid? BrandId, 
    Guid? TaxId, 
    string CabysCode, 
    string MeasurementUnit,
    List<CreateProductVariantDto> Variants) : IRequest<Guid>;
