namespace NexusERP.WebDashboard.Models.Sales;

public class SaleSummaryDto
{
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class CreateSaleCommand
{
    public Guid CashRegisterSessionId { get; set; }
    public Guid BranchId { get; set; }
    public Guid? CustomerId { get; set; }
    
    public List<SaleDetailModel> Details { get; set; } = new();
    public List<PaymentModel> Payments { get; set; } = new();
}

public class SaleDetailModel
{
    public Guid ProductVariantId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    
    public decimal Subtotal => Quantity * UnitPrice;
    public decimal TaxTotal => Subtotal * (TaxPercentage / 100m);
    public decimal Total => Subtotal + TaxTotal;
}

public class PaymentModel
{
    public string PaymentMethod { get; set; } = "Efectivo";
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
}
