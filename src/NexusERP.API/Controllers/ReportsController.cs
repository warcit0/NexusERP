using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusERP.Application.Reports.Queries.GetSalesReport;
using NexusERP.Application.Reports.Queries.GetCriticalInventoryReport;
using NexusERP.Application.Reports.Queries.GetDashboardSummary;
using NexusERP.Application.Reports.Queries.GetReceivablesAging;
using ClosedXML.Excel;
using System.IO;

namespace NexusERP.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardSummaryDto>> GetDashboard()
    {
        return Ok(await _mediator.Send(new GetDashboardSummaryQuery()));
    }

    [HttpGet("sales")]
    public async Task<ActionResult<List<SalesReportItemDto>>> GetSales([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        return Ok(await _mediator.Send(new GetSalesReportQuery(startDate, endDate)));
    }

    [HttpGet("sales/export/excel")]
    public async Task<IActionResult> ExportSalesToExcel([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var sales = await _mediator.Send(new GetSalesReportQuery(startDate, endDate));

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Ventas");
        var currentRow = 1;

        // Headers
        worksheet.Cell(currentRow, 1).Value = "Fecha";
        worksheet.Cell(currentRow, 2).Value = "Sucursal";
        worksheet.Cell(currentRow, 3).Value = "Cantidad Ventas";
        worksheet.Cell(currentRow, 4).Value = "Ingresos Totales";

        // Formato Headers
        worksheet.Range("A1:D1").Style.Font.Bold = true;
        worksheet.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.LightGray;

        // Datos
        foreach (var sale in sales)
        {
            currentRow++;
            worksheet.Cell(currentRow, 1).Value = sale.Date.ToString("yyyy-MM-dd");
            worksheet.Cell(currentRow, 2).Value = sale.BranchName;
            worksheet.Cell(currentRow, 3).Value = sale.TotalSales;
            worksheet.Cell(currentRow, 4).Value = sale.TotalRevenue;
            
            // Formato moneda
            worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "$#,##0.00";
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        var content = stream.ToArray();

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ReporteVentas_{DateTime.Now:yyyyMMdd}.xlsx");
    }

    [HttpGet("inventory/critical")]
    public async Task<ActionResult<List<CriticalInventoryDto>>> GetCriticalInventory([FromQuery] decimal threshold = 10)
    {
        return Ok(await _mediator.Send(new GetCriticalInventoryReportQuery(threshold)));
    }

    [HttpGet("receivables/aging")]
    public async Task<ActionResult<ReceivablesAgingDto>> GetReceivablesAging()
    {
        return Ok(await _mediator.Send(new GetReceivablesAgingQuery()));
    }
}
