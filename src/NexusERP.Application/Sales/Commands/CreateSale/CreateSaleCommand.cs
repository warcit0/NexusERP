using MediatR;

namespace NexusERP.Application.Sales.Commands.CreateSale;

public class CreateSaleCommand : IRequest<Guid>
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
}

public class PaymentModel
{
    public string PaymentMethod { get; set; } = string.Empty; // Efectivo, Tarjeta, etc.
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
}
