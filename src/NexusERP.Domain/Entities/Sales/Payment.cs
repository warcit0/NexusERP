namespace NexusERP.Domain.Entities.Sales;

public class Payment : TenantEntity
{
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    // Efectivo, Tarjeta, Transferencia, SINPE
    public string PaymentMethod { get; set; } = "Efectivo"; 
    
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty; // Ej. Número de comprobante de tarjeta
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
}
