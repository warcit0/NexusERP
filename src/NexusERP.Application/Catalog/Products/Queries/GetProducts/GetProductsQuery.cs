using MediatR;

namespace NexusERP.Application.Catalog.Products.Queries.GetProducts;

public record ProductVariantDto(Guid Id, string Sku, string Barcode, string Size, string Color, decimal Cost, decimal Price, bool IsActive);

public record ProductDto(
    Guid Id, 
    string Name, 
    string Description, 
    string CategoryName, 
    string? BrandName,
    string? TaxName,
    string CabysCode,
    string MeasurementUnit,
    bool IsActive,
    List<ProductVariantDto> Variants);

public record GetProductsQuery : IRequest<List<ProductDto>>;
