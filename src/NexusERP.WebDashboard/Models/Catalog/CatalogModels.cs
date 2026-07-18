namespace NexusERP.WebDashboard.Models.Catalog;

public class ProductVariantDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; } = 13;
}

public class CashRegisterSessionDto
{
    public Guid Id { get; set; }
    public Guid CashRegisterId { get; set; }
    public Guid BranchId { get; set; }
    public decimal InitialAmount { get; set; }
    public DateTime OpenedAt { get; set; }
    public string OpenedByUserId { get; set; } = string.Empty;
}

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = new();
}
