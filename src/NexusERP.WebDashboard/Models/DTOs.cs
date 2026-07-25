using System;

namespace NexusERP.WebDashboard.Models;

public class AccountsReceivableDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class AccountsPayableDto
{
    public Guid Id { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class SalesReportItemDto
{
    public DateTime Date { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
}
