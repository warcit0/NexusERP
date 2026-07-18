using MediatR;

namespace NexusERP.Application.Inventory.Queries.GetInventoryBalances;

public class GetInventoryBalancesQuery : IRequest<List<InventoryBalanceDto>>
{
    public Guid? BranchId { get; set; }
}

public class InventoryBalanceDto
{
    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public DateTime LastUpdated { get; set; }
}
