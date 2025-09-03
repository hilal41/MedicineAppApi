using Microsoft.AspNetCore.Mvc;
using MedicineAppApi.DTOs;
using MedicineAppApi.Services;

namespace MedicineAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        // Stock Reports
        // GET: api/reports/stock/low-stock?threshold=10
        [HttpGet("stock/low-stock")]
        public async Task<ActionResult<IEnumerable<LowStockReportDto>>> GetLowStockReport([FromQuery] int threshold = 10)
        {
            var report = await _reportsService.GetLowStockReportAsync(threshold);
            return Ok(report);
        }

        // GET: api/reports/stock/expired
        [HttpGet("stock/expired")]
        public async Task<ActionResult<IEnumerable<ExpiredMedicineReportDto>>> GetExpiredMedicinesReport()
        {
            var report = await _reportsService.GetExpiredMedicinesReportAsync();
            return Ok(report);
        }

        // GET: api/reports/stock/expiring-soon?daysAhead=30
        [HttpGet("stock/expiring-soon")]
        public async Task<ActionResult<IEnumerable<ExpiringSoonReportDto>>> GetExpiringSoonReport([FromQuery] int daysAhead = 30)
        {
            var report = await _reportsService.GetExpiringSoonReportAsync(daysAhead);
            return Ok(report);
        }

        // GET: api/reports/stock/movements?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("stock/movements")]
        public async Task<ActionResult<IEnumerable<StockMovementReportDto>>> GetStockMovementReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetStockMovementReportAsync(startDate, endDate);
            return Ok(report);
        }

        // Sales Reports
        // GET: api/reports/sales/summary?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("sales/summary")]
        public async Task<ActionResult<SalesSummaryReportDto>> GetSalesSummaryReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetSalesSummaryReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/sales/by-date?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("sales/by-date")]
        public async Task<ActionResult<IEnumerable<SalesByDateReportDto>>> GetSalesByDateReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetSalesByDateReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/sales/by-medicine?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("sales/by-medicine")]
        public async Task<ActionResult<IEnumerable<SalesByMedicineReportDto>>> GetSalesByMedicineReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetSalesByMedicineReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/sales/by-customer?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("sales/by-customer")]
        public async Task<ActionResult<IEnumerable<SalesByCustomerReportDto>>> GetSalesByCustomerReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetSalesByCustomerReportAsync(startDate, endDate);
            return Ok(report);
        }

        // Purchase Reports
        // GET: api/reports/purchases/summary?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("purchases/summary")]
        public async Task<ActionResult<PurchaseSummaryReportDto>> GetPurchaseSummaryReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetPurchaseSummaryReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/purchases/by-supplier?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("purchases/by-supplier")]
        public async Task<ActionResult<IEnumerable<PurchaseBySupplierReportDto>>> GetPurchaseBySupplierReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetPurchaseBySupplierReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/purchases/by-medicine?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("purchases/by-medicine")]
        public async Task<ActionResult<IEnumerable<PurchaseByMedicineReportDto>>> GetPurchaseByMedicineReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetPurchaseByMedicineReportAsync(startDate, endDate);
            return Ok(report);
        }

        // Financial Reports
        // GET: api/reports/financial/summary?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("financial/summary")]
        public async Task<ActionResult<FinancialSummaryReportDto>> GetFinancialSummaryReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetFinancialSummaryReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/financial/payment-methods?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("financial/payment-methods")]
        public async Task<ActionResult<IEnumerable<PaymentMethodReportDto>>> GetPaymentMethodReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetPaymentMethodReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/financial/profit-loss?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("financial/profit-loss")]
        public async Task<ActionResult<ProfitLossReportDto>> GetProfitLossReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetProfitLossReportAsync(startDate, endDate);
            return Ok(report);
        }

        // Inventory Reports
        // GET: api/reports/inventory/value
        [HttpGet("inventory/value")]
        public async Task<ActionResult<InventoryValueReportDto>> GetInventoryValueReport()
        {
            var report = await _reportsService.GetInventoryValueReportAsync();
            return Ok(report);
        }

        // GET: api/reports/inventory/medicine-performance?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("inventory/medicine-performance")]
        public async Task<ActionResult<IEnumerable<MedicinePerformanceReportDto>>> GetMedicinePerformanceReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var report = await _reportsService.GetMedicinePerformanceReportAsync(startDate, endDate);
            return Ok(report);
        }

        // Dashboard Reports (Combined)
        // GET: api/reports/dashboard?startDate=2024-01-01&endDate=2024-01-31
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboardReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            // Get all key metrics for dashboard
            var salesSummary = await _reportsService.GetSalesSummaryReportAsync(startDate, endDate);
            var purchaseSummary = await _reportsService.GetPurchaseSummaryReportAsync(startDate, endDate);
            var financialSummary = await _reportsService.GetFinancialSummaryReportAsync(startDate, endDate);
            var inventoryValue = await _reportsService.GetInventoryValueReportAsync();
            var lowStockItems = await _reportsService.GetLowStockReportAsync(10);
            var expiringSoonItems = await _reportsService.GetExpiringSoonReportAsync(30);

            return Ok(new
            {
                Period = new { StartDate = startDate, EndDate = endDate },
                Sales = new
                {
                    TotalSales = salesSummary.TotalSales,
                    TotalInvoices = salesSummary.TotalInvoices,
                    AverageSale = salesSummary.AverageSale
                },
                Purchases = new
                {
                    TotalPurchases = purchaseSummary.TotalPurchases,
                    TotalPurchaseCount = purchaseSummary.TotalPurchaseCount,
                    AveragePurchase = purchaseSummary.AveragePurchase
                },
                Financial = new
                {
                    GrossProfit = financialSummary.GrossProfit,
                    ProfitMargin = financialSummary.ProfitMargin,
                    OutstandingReceivables = financialSummary.OutstandingReceivables
                },
                Inventory = new
                {
                    TotalValue = inventoryValue.TotalInventoryValue,
                    TotalItems = inventoryValue.TotalItems,
                    UniqueProducts = inventoryValue.UniqueProducts
                },
                Alerts = new
                {
                    LowStockCount = lowStockItems.Count(),
                    ExpiringSoonCount = expiringSoonItems.Count()
                }
            });
        }

        // Quick Reports (Last 30 days by default)
        // GET: api/reports/quick/sales
        [HttpGet("quick/sales")]
        public async Task<ActionResult<SalesSummaryReportDto>> GetQuickSalesReport()
        {
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            var report = await _reportsService.GetSalesSummaryReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/quick/purchases
        [HttpGet("quick/purchases")]
        public async Task<ActionResult<PurchaseSummaryReportDto>> GetQuickPurchaseReport()
        {
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            var report = await _reportsService.GetPurchaseSummaryReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/quick/financial
        [HttpGet("quick/financial")]
        public async Task<ActionResult<FinancialSummaryReportDto>> GetQuickFinancialReport()
        {
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            var report = await _reportsService.GetFinancialSummaryReportAsync(startDate, endDate);
            return Ok(report);
        }

        // GET: api/reports/quick/alerts
        [HttpGet("quick/alerts")]
        public async Task<ActionResult<object>> GetQuickAlertsReport()
        {
            var lowStockItems = await _reportsService.GetLowStockReportAsync(10);
            var expiredItems = await _reportsService.GetExpiredMedicinesReportAsync();
            var expiringSoonItems = await _reportsService.GetExpiringSoonReportAsync(30);

            return Ok(new
            {
                LowStock = new
                {
                    Count = lowStockItems.Count(),
                    Items = lowStockItems.Take(5).ToList() // Top 5 for quick view
                },
                Expired = new
                {
                    Count = expiredItems.Count(),
                    Items = expiredItems.Take(5).ToList() // Top 5 for quick view
                },
                ExpiringSoon = new
                {
                    Count = expiringSoonItems.Count(),
                    Items = expiringSoonItems.Take(5).ToList() // Top 5 for quick view
                }
            });
        }
    }
}
