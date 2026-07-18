using MediatR;

namespace NexusERP.Application.Catalog.Products.Commands.UpdateProductVariant;

public class UpdateProductVariantCommand : IRequest<bool>
{
    public Guid VariantId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public decimal Price { get; set; }
}
