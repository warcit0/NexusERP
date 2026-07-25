using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusERP.Application.Reports.Queries.GetCriticalInventoryReport;

public record GetCriticalInventoryReportQuery(decimal Threshold) : IRequest<List<CriticalInventoryDto>>;

public record CriticalInventoryDto(
    Guid ProductVariantId,
    string ProductName,
    string Sku,
    decimal CurrentStock,
    decimal Threshold
);

public class GetCriticalInventoryReportQueryHandler : IRequestHandler<GetCriticalInventoryReportQuery, List<CriticalInventoryDto>>
{
    private readonly INexusDbContext _context;

    public GetCriticalInventoryReportQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<List<CriticalInventoryDto>> Handle(GetCriticalInventoryReportQuery request, CancellationToken cancellationToken)
    {
        var criticalItems = await _context.InventoryBalances
            .Include(ib => ib.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Where(ib => ib.CurrentStock <= request.Threshold)
            .Select(ib => new CriticalInventoryDto(
                ib.ProductVariantId,
                ib.ProductVariant != null && ib.ProductVariant.Product != null ? ib.ProductVariant.Product.Name : "Desconocido",
                ib.ProductVariant != null ? ib.ProductVariant.Sku : "Sin SKU",
                ib.CurrentStock,
                request.Threshold
            ))
            .ToListAsync(cancellationToken);

        return criticalItems;
    }
}
