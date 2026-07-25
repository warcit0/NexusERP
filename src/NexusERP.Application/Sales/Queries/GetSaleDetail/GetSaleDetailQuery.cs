using MediatR;
using Microsoft.EntityFrameworkCore;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Application.Sales.Queries.GetSaleDetail;

public record GetSaleDetailQuery(Guid SaleId) : IRequest<SaleDetailDto?>;

public class SaleDetailDto
{
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public List<SaleLineDto> Lines { get; set; } = new();
    public List<SalePaymentDto> Payments { get; set; } = new();
}

public class SaleLineDto
{
    public string ProductName { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
}

public class SalePaymentDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Reference { get; set; } = string.Empty;
}

public class GetSaleDetailQueryHandler : IRequestHandler<GetSaleDetailQuery, SaleDetailDto?>
{
    private readonly INexusDbContext _context;

    public GetSaleDetailQueryHandler(INexusDbContext context)
    {
        _context = context;
    }

    public async Task<SaleDetailDto?> Handle(GetSaleDetailQuery request, CancellationToken cancellationToken)
    {
        var sale = await _context.Sales
            .Include(s => s.Details)
            .Include(s => s.Payments)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == request.SaleId, cancellationToken);

        if (sale == null) return null;

        return new SaleDetailDto
        {
            Id = sale.Id,
            ReceiptNumber = sale.ReceiptNumber,
            Date = sale.Date,
            CustomerName = sale.Customer?.Name ?? "Consumidor Final",
            Status = sale.Status,
            Subtotal = sale.Subtotal,
            TaxTotal = sale.TaxTotal,
            Total = sale.Total,
            Lines = sale.Details.Select(d => new SaleLineDto
            {
                ProductName = d.ProductName,
                Sku = d.Sku,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                TaxPercentage = d.TaxPercentage,
                Subtotal = d.Subtotal,
                Total = d.Total
            }).ToList(),
            Payments = sale.Payments.Select(p => new SalePaymentDto
            {
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                Reference = p.Reference ?? string.Empty
            }).ToList()
        };
    }
}
