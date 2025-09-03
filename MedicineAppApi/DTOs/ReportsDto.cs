namespace MedicineAppApi.DTOs
{
    // Stock Reports
    public class LowStockReportDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }

    public class ExpiredMedicineReportDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysExpired { get; set; }
        public decimal ValueAtRisk { get; set; }
    }

    public class ExpiringSoonReportDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int DaysUntilExpiry { get; set; }
        public decimal ValueAtRisk { get; set; }
        public string Urgency { get; set; } = string.Empty;
    }

    public class StockMovementReportDto
    {
        public int MovementId { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public int ChangeQuantity { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string ReferenceType { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    // Sales Reports
    public class SalesSummaryReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public int TotalInvoices { get; set; }
        public decimal AverageSale { get; set; }
        public List<DailySalesDto> DailySales { get; set; } = new List<DailySalesDto>();
    }

    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int InvoiceCount { get; set; }
        public decimal AverageSale { get; set; }
    }

    public class SalesByDateReportDto
    {
        public DateTime Date { get; set; }
        public decimal TotalSales { get; set; }
        public int InvoiceCount { get; set; }
        public decimal AverageSale { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal NetSales { get; set; }
    }

    public class SalesByMedicineReportDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePrice { get; set; }
        public int SaleCount { get; set; }
    }

    public class SalesByCustomerReportDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public decimal TotalPurchases { get; set; }
        public int InvoiceCount { get; set; }
        public decimal AveragePurchase { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }

    // Purchase Reports
    public class PurchaseSummaryReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPurchases { get; set; }
        public int TotalPurchaseCount { get; set; }
        public decimal AveragePurchase { get; set; }
        public List<DailyPurchaseDto> DailyPurchases { get; set; } = new List<DailyPurchaseDto>();
    }

    public class DailyPurchaseDto
    {
        public DateTime Date { get; set; }
        public decimal TotalPurchases { get; set; }
        public int PurchaseCount { get; set; }
        public decimal AveragePurchase { get; set; }
    }

    public class PurchaseBySupplierReportDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string SupplierContact { get; set; } = string.Empty;
        public decimal TotalPurchases { get; set; }
        public int PurchaseCount { get; set; }
        public decimal AveragePurchase { get; set; }
        public DateTime LastPurchaseDate { get; set; }
    }

    public class PurchaseByMedicineReportDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int TotalQuantityPurchased { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AverageCost { get; set; }
        public int PurchaseCount { get; set; }
    }

    // Financial Reports
    public class FinancialSummaryReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalPurchases { get; set; }
        public decimal TotalPayments { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal OutstandingReceivables { get; set; }
    }

    public class PaymentMethodReportDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int PaymentCount { get; set; }
        public decimal AverageAmount { get; set; }
        public decimal PercentageOfTotal { get; set; }
    }

    public class ProfitLossReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Revenue { get; set; }
        public decimal CostOfGoodsSold { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal CashFlow { get; set; }
    }

    // Inventory Reports
    public class InventoryValueReportDto
    {
        public decimal TotalInventoryValue { get; set; }
        public int TotalItems { get; set; }
        public int UniqueProducts { get; set; }
        public decimal AverageItemValue { get; set; }
        public List<CategoryInventoryDto> ByCategory { get; set; } = new List<CategoryInventoryDto>();
    }

    public class CategoryInventoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public int TotalItems { get; set; }
        public int ProductCount { get; set; }
    }

    public class MedicinePerformanceReportDto
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public string MedicineBarcode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CurrentStock { get; set; }
        public int TotalSold { get; set; }
        public int TotalPurchased { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }
        public decimal ProfitMargin { get; set; }
        public double TurnoverRate { get; set; }
    }
}
