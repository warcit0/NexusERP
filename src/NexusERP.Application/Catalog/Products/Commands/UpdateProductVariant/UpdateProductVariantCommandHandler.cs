using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Catalog.Products.Commands.UpdateProductVariant;

public class UpdateProductVariantCommandHandler : IRequestHandler<UpdateProductVariantCommand, bool>
{
    private readonly INexusDbContext _context;

    public UpdateProductVariantCommandHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
    {
        var variant = await _context.ProductVariants
            .FirstOrDefaultAsync(v => v.Id == request.VariantId, cancellationToken);

        if (variant == null)
            throw new Exception($"Variante {request.VariantId} no encontrada.");

        variant.Sku = request.Sku;
        variant.Barcode = request.Barcode;
        variant.Size = request.Size;
        variant.Color = request.Color;
        variant.Cost = request.Cost;
        variant.Price = request.Price;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
