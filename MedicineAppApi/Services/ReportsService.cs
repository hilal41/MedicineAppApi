using MedicineAppApi.Models;
using MedicineAppApi.DTOs;
using MedicineAppApi.Repositories.Interfaces;
using AutoMapper;

namespace MedicineAppApi.Services
{
    public interface IReportsService
    {
        // Stock Reports
        Task<IEnumerable<LowStockReportDto>> GetLowStockReportAsync(int threshold = 10);
        Task<IEnumerable<ExpiredMedicineReportDto>> GetExpiredMedicinesReportAsync();
        Task<IEnumerable<ExpiringSoonReportDto>> GetExpiringSoonReportAsync(int daysAhead = 30);
        Task<IEnumerable<StockMovementReportDto>> GetStockMovementReportAsync(DateTime startDate, DateTime endDate);
        
        // Sales Reports
        Task<SalesSummaryReportDto> GetSalesSummaryReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SalesByDateReportDto>> GetSalesByDateReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SalesByMedicineReportDto>> GetSalesByMedicineReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SalesByCustomerReportDto>> GetSalesByCustomerReportAsync(DateTime startDate, DateTime endDate);
        
        // Purchase Reports
        Task<PurchaseSummaryReportDto> GetPurchaseSummaryReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PurchaseBySupplierReportDto>> GetPurchaseBySupplierReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PurchaseByMedicineReportDto>> GetPurchaseByMedicineReportAsync(DateTime startDate, DateTime endDate);
        
        // Financial Reports
        Task<FinancialSummaryReportDto> GetFinancialSummaryReportAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentMethodReportDto>> GetPaymentMethodReportAsync(DateTime startDate, DateTime endDate);
        Task<ProfitLossReportDto> GetProfitLossReportAsync(DateTime startDate, DateTime endDate);
        
        // Inventory Reports
        Task<InventoryValueReportDto> GetInventoryValueReportAsync();
        Task<IEnumerable<MedicinePerformanceReportDto>> GetMedicinePerformanceReportAsync(DateTime startDate, DateTime endDate);
    }

    public class ReportsService : IReportsService
    {
        private readonly IMedicineRepository _medicineRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceItemRepository _invoiceItemRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IPurchaseItemRepository _purchaseItemRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IStockMovementRepository _stockMovementRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public ReportsService(
            IMedicineRepository medicineRepository,
            IInvoiceRepository invoiceRepository,
            IInvoiceItemRepository invoiceItemRepository,
            IPurchaseRepository purchaseRepository,
            IPurchaseItemRepository purchaseItemRepository,
            IPaymentRepository paymentRepository,
            IStockMovementRepository stockMovementRepository,
            ISupplierRepository supplierRepository,
            ICustomerRepository customerRepository,
            IMapper mapper)
        {
            _medicineRepository = medicineRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceItemRepository = invoiceItemRepository;
            _purchaseRepository = purchaseRepository;
            _purchaseItemRepository = purchaseItemRepository;
            _paymentRepository = paymentRepository;
            _stockMovementRepository = stockMovementRepository;
            _supplierRepository = supplierRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        // Stock Reports
        public async Task<IEnumerable<LowStockReportDto>> GetLowStockReportAsync(int threshold = 10)
        {
            var medicines = await _medicineRepository.GetAllAsync();
            var lowStockMedicines = medicines.Where(m => m.Quantity <= threshold)
                                            .OrderBy(m => m.Quantity)
                                            .ThenBy(m => m.Name);

            var report = lowStockMedicines.Select(m => new LowStockReportDto
            {
                MedicineId = m.Id,
                MedicineName = m.Name,
                MedicineBarcode = m.Barcode ?? "",
                CategoryName = "", // Will be populated if needed
                CurrentStock = m.Quantity,
                Threshold = threshold,
                Status = m.Quantity == 0 ? "Out of Stock" : "Low Stock",
                LastUpdated = m.UpdatedAt
            });

            return report;
        }

        public async Task<IEnumerable<ExpiredMedicineReportDto>> GetExpiredMedicinesReportAsync()
        {
            var medicines = await _medicineRepository.GetAllAsync();
            var expiredMedicines = medicines.Where(m => m.ExpiryDate <= DateTime.UtcNow)
                                           .OrderBy(m => m.ExpiryDate)
                                           .ThenBy(m => m.Name);

            var report = expiredMedicines.Select(m => new ExpiredMedicineReportDto
            {
                MedicineId = m.Id,
                MedicineName = m.Name,
                MedicineBarcode = m.Barcode ?? "",
                CategoryName = "", // Will be populated if needed
                CurrentStock = m.Quantity,
                ExpiryDate = m.ExpiryDate,
                DaysExpired = (DateTime.UtcNow - m.ExpiryDate).Days,
                ValueAtRisk = m.Quantity * m.Price
            });

            return report;
        }

        public async Task<IEnumerable<ExpiringSoonReportDto>> GetExpiringSoonReportAsync(int daysAhead = 30)
        {
            var medicines = await _medicineRepository.GetAllAsync();
            var targetDate = DateTime.UtcNow.AddDays(daysAhead);
            var expiringSoon = medicines.Where(m => m.ExpiryDate <= targetDate && m.ExpiryDate > DateTime.UtcNow)
                                       .OrderBy(m => m.ExpiryDate)
                                       .ThenBy(m => m.Name);

            var report = expiringSoon.Select(m => new ExpiringSoonReportDto
            {
                MedicineId = m.Id,
                MedicineName = m.Name,
                MedicineBarcode = m.Barcode ?? "",
                CategoryName = "", // Will be populated if needed
                CurrentStock = m.Quantity,
                ExpiryDate = m.ExpiryDate,
                DaysUntilExpiry = (m.ExpiryDate - DateTime.UtcNow).Days,
                ValueAtRisk = m.Quantity * m.Price,
                Urgency = (m.ExpiryDate - DateTime.UtcNow).Days <= 7 ? "High" : 
                         (m.ExpiryDate - DateTime.UtcNow).Days <= 14 ? "Medium" : "Low"
            });

            return report;
        }

        public async Task<IEnumerable<StockMovementReportDto>> GetStockMovementReportAsync(DateTime startDate, DateTime endDate)
        {
            var movements = await _stockMovementRepository.GetByDateRangeAsync(startDate, endDate);
            
            var report = movements.Select(m => new StockMovementReportDto
            {
                MovementId = m.Id,
                MedicineId = m.MedicineId,
                MedicineName = "", // Will be populated if needed
                MedicineBarcode = "", // Will be populated if needed
                ChangeQuantity = m.ChangeQty,
                MovementType = m.ChangeQty > 0 ? "Stock In" : "Stock Out",
                Reason = m.Reason,
                ReferenceType = m.ReferenceType,
                ReferenceId = m.ReferenceId ?? 0,
                CreatedBy = m.CreatedBy,
                CreatedByUserName = "", // Will be populated if needed
                CreatedAt = m.CreatedAt
            });

            return report.OrderByDescending(r => r.CreatedAt);
        }

        // Sales Reports
        public async Task<SalesSummaryReportDto> GetSalesSummaryReportAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _invoiceRepository.GetByDateRangeAsync(startDate, endDate);
            var totalSales = invoices.Sum(i => i.Total);
            var totalInvoices = invoices.Count();
            var averageSale = totalInvoices > 0 ? totalSales / totalInvoices : 0;

            var salesByDay = invoices.GroupBy(i => i.Date.Date)
                                    .Select(g => new DailySalesDto
                                    {
                                        Date = g.Key,
                                        TotalSales = g.Sum(i => i.Total),
                                        InvoiceCount = g.Count(),
                                        AverageSale = g.Count() > 0 ? g.Sum(i => i.Total) / g.Count() : 0
                                    })
                                    .OrderBy(s => s.Date)
                                    .ToList();

            return new SalesSummaryReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = totalSales,
                TotalInvoices = totalInvoices,
                AverageSale = averageSale,
                DailySales = salesByDay
            };
        }

        public async Task<IEnumerable<SalesByDateReportDto>> GetSalesByDateReportAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _invoiceRepository.GetByDateRangeAsync(startDate, endDate);
            
            var report = invoices.GroupBy(i => i.Date.Date)
                                .Select(g => new SalesByDateReportDto
                                {
                                    Date = g.Key,
                                    TotalSales = g.Sum(i => i.Total),
                                    InvoiceCount = g.Count(),
                                    AverageSale = g.Count() > 0 ? g.Sum(i => i.Total) / g.Count() : 0,
                                    TotalDiscount = g.Sum(i => i.DiscountAmount),
                                    NetSales = g.Sum(i => i.Subtotal)
                                })
                                .OrderByDescending(r => r.Date);

            return report;
        }

        public async Task<IEnumerable<SalesByMedicineReportDto>> GetSalesByMedicineReportAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _invoiceRepository.GetByDateRangeAsync(startDate, endDate);
            var invoiceIds = invoices.Select(i => i.Id).ToList();
            
            // Get all invoice items for these invoices
            var allInvoiceItems = new List<InvoiceItem>();
            foreach (var invoiceId in invoiceIds)
            {
                var items = await _invoiceItemRepository.GetByInvoiceAsync(invoiceId);
                allInvoiceItems.AddRange(items);
            }

            var report = allInvoiceItems.GroupBy(ii => ii.MedicineId)
                                       .Select(g => new SalesByMedicineReportDto
                                       {
                                           MedicineId = g.Key,
                                           MedicineName = "", // Will be populated if needed
                                           MedicineBarcode = "", // Will be populated if needed
                                           CategoryName = "", // Will be populated if needed
                                           TotalQuantitySold = g.Sum(ii => ii.Qty),
                                           TotalRevenue = g.Sum(ii => ii.Subtotal),
                                           AveragePrice = g.Sum(ii => ii.Subtotal) / g.Sum(ii => ii.Qty),
                                           SaleCount = g.Count()
                                       })
                                       .OrderByDescending(r => r.TotalRevenue);

            return report;
        }

        public async Task<IEnumerable<SalesByCustomerReportDto>> GetSalesByCustomerReportAsync(DateTime startDate, DateTime endDate)
        {
            var invoices = await _invoiceRepository.GetByDateRangeAsync(startDate, endDate);
            
            var report = invoices.GroupBy(i => i.CustomerId)
                                .Select(g => new SalesByCustomerReportDto
                                {
                                    CustomerId = g.Key,
                                    CustomerName = "", // Will be populated if needed
                                    CustomerPhone = "", // Will be populated if needed
                                    TotalPurchases = g.Sum(i => i.Total),
                                    InvoiceCount = g.Count(),
                                    AveragePurchase = g.Count() > 0 ? g.Sum(i => i.Total) / g.Count() : 0,
                                    LastPurchaseDate = g.Max(i => i.Date)
                                })
                                .OrderByDescending(r => r.TotalPurchases);

            return report;
        }

        // Purchase Reports
        public async Task<PurchaseSummaryReportDto> GetPurchaseSummaryReportAsync(DateTime startDate, DateTime endDate)
        {
            var purchases = await _purchaseRepository.GetByDateRangeAsync(startDate, endDate);
            var totalPurchases = purchases.Sum(p => p.Total);
            var totalPurchaseCount = purchases.Count();
            var averagePurchase = totalPurchaseCount > 0 ? totalPurchases / totalPurchaseCount : 0;

            var purchasesByDay = purchases.GroupBy(p => p.Date.Date)
                                        .Select(g => new DailyPurchaseDto
                                        {
                                            Date = g.Key,
                                            TotalPurchases = g.Sum(p => p.Total),
                                            PurchaseCount = g.Count(),
                                            AveragePurchase = g.Count() > 0 ? g.Sum(p => p.Total) / g.Count() : 0
                                        })
                                        .OrderBy(p => p.Date)
                                        .ToList();

            return new PurchaseSummaryReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalPurchases = totalPurchases,
                TotalPurchaseCount = totalPurchaseCount,
                AveragePurchase = averagePurchase,
                DailyPurchases = purchasesByDay
            };
        }

        public async Task<IEnumerable<PurchaseBySupplierReportDto>> GetPurchaseBySupplierReportAsync(DateTime startDate, DateTime endDate)
        {
            var purchases = await _purchaseRepository.GetByDateRangeAsync(startDate, endDate);
            
            var report = purchases.GroupBy(p => p.SupplierId)
                                 .Select(g => new PurchaseBySupplierReportDto
                                 {
                                     SupplierId = g.Key,
                                     SupplierName = "", // Will be populated if needed
                                     SupplierContact = "", // Will be populated if needed
                                     TotalPurchases = g.Sum(p => p.Total),
                                     PurchaseCount = g.Count(),
                                     AveragePurchase = g.Count() > 0 ? g.Sum(p => p.Total) / g.Count() : 0,
                                     LastPurchaseDate = g.Max(p => p.Date)
                                 })
                                 .OrderByDescending(r => r.TotalPurchases);

            return report;
        }

        public async Task<IEnumerable<PurchaseByMedicineReportDto>> GetPurchaseByMedicineReportAsync(DateTime startDate, DateTime endDate)
        {
            var purchases = await _purchaseRepository.GetByDateRangeAsync(startDate, endDate);
            var purchaseIds = purchases.Select(p => p.Id).ToList();
            
            // Get all purchase items for these purchases
            var allPurchaseItems = new List<PurchaseItem>();
            foreach (var purchaseId in purchaseIds)
            {
                var items = await _purchaseItemRepository.GetByPurchaseAsync(purchaseId);
                allPurchaseItems.AddRange(items);
            }

            var report = allPurchaseItems.GroupBy(pi => pi.MedicineId)
                                       .Select(g => new PurchaseByMedicineReportDto
                                       {
                                           MedicineId = g.Key,
                                           MedicineName = "", // Will be populated if needed
                                           MedicineBarcode = "", // Will be populated if needed
                                           CategoryName = "", // Will be populated if needed
                                           TotalQuantityPurchased = g.Sum(pi => pi.Qty),
                                           TotalCost = g.Sum(pi => pi.Subtotal),
                                           AverageCost = g.Sum(pi => pi.Subtotal) / g.Sum(pi => pi.Qty),
                                           PurchaseCount = g.Count()
                                       })
                                       .OrderByDescending(r => r.TotalCost);

            return report;
        }

        // Financial Reports
        public async Task<FinancialSummaryReportDto> GetFinancialSummaryReportAsync(DateTime startDate, DateTime endDate)
        {
            var totalSales = await _invoiceRepository.GetTotalSalesAsync(startDate, endDate);
            var totalPurchases = await _purchaseRepository.GetTotalPurchasesAsync(startDate, endDate);
            var totalPayments = await _paymentRepository.GetTotalPaymentsByDateRangeAsync(startDate, endDate);
            
            var grossProfit = totalSales - totalPurchases;
            var profitMargin = totalSales > 0 ? (grossProfit / totalSales) * 100 : 0;

            return new FinancialSummaryReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalSales = totalSales,
                TotalPurchases = totalPurchases,
                TotalPayments = totalPayments,
                GrossProfit = grossProfit,
                ProfitMargin = profitMargin,
                OutstandingReceivables = totalSales - totalPayments
            };
        }

        public async Task<IEnumerable<PaymentMethodReportDto>> GetPaymentMethodReportAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
            
            var report = payments.GroupBy(p => p.Method)
                                .Select(g => new PaymentMethodReportDto
                                {
                                    PaymentMethod = g.Key,
                                    TotalAmount = g.Sum(p => p.Amount),
                                    PaymentCount = g.Count(),
                                    AverageAmount = g.Count() > 0 ? g.Sum(p => p.Amount) / g.Count() : 0,
                                    PercentageOfTotal = 0 // Will be calculated after getting total
                                })
                                .OrderByDescending(r => r.TotalAmount)
                                .ToList();

            var totalPayments = report.Sum(r => r.TotalAmount);
            foreach (var item in report)
            {
                item.PercentageOfTotal = totalPayments > 0 ? (item.TotalAmount / totalPayments) * 100 : 0;
            }

            return report;
        }

        public async Task<ProfitLossReportDto> GetProfitLossReportAsync(DateTime startDate, DateTime endDate)
        {
            var totalSales = await _invoiceRepository.GetTotalSalesAsync(startDate, endDate);
            var totalPurchases = await _purchaseRepository.GetTotalPurchasesAsync(startDate, endDate);
            var totalPayments = await _paymentRepository.GetTotalPaymentsByDateRangeAsync(startDate, endDate);
            
            var grossProfit = totalSales - totalPurchases;
            var netProfit = totalPayments - totalPurchases; // Simplified calculation
            var profitMargin = totalSales > 0 ? (grossProfit / totalSales) * 100 : 0;

            return new ProfitLossReportDto
            {
                StartDate = startDate,
                EndDate = endDate,
                Revenue = totalSales,
                CostOfGoodsSold = totalPurchases,
                GrossProfit = grossProfit,
                NetProfit = netProfit,
                ProfitMargin = profitMargin,
                CashFlow = totalPayments - totalPurchases
            };
        }

        // Inventory Reports
        public async Task<InventoryValueReportDto> GetInventoryValueReportAsync()
        {
            var medicines = await _medicineRepository.GetAllAsync();
            
            var totalValue = medicines.Sum(m => m.Quantity * m.Price);
            var totalItems = medicines.Sum(m => m.Quantity);
            var uniqueProducts = medicines.Count();

            var byCategory = medicines.GroupBy(m => "Uncategorized") // Will be populated if needed
                                    .Select(g => new CategoryInventoryDto
                                    {
                                        CategoryName = g.Key,
                                        TotalValue = g.Sum(m => m.Quantity * m.Price),
                                        TotalItems = g.Sum(m => m.Quantity),
                                        ProductCount = g.Count()
                                    })
                                    .OrderByDescending(c => c.TotalValue)
                                    .ToList();

            return new InventoryValueReportDto
            {
                TotalInventoryValue = totalValue,
                TotalItems = totalItems,
                UniqueProducts = uniqueProducts,
                AverageItemValue = totalItems > 0 ? totalValue / totalItems : 0,
                ByCategory = byCategory
            };
        }

        public async Task<IEnumerable<MedicinePerformanceReportDto>> GetMedicinePerformanceReportAsync(DateTime startDate, DateTime endDate)
        {
            var medicines = await _medicineRepository.GetAllAsync();
            var performanceReports = new List<MedicinePerformanceReportDto>();

            foreach (var medicine in medicines)
            {
                // Get sales data
                var salesItems = await _invoiceItemRepository.GetByMedicineAsync(medicine.Id);
                var salesInPeriod = salesItems.Where(ii => 
                    ii.Invoice?.Date >= startDate && ii.Invoice?.Date <= endDate).ToList();

                var totalSold = salesInPeriod.Sum(ii => ii.Qty);
                var totalRevenue = salesInPeriod.Sum(ii => ii.Subtotal);

                // Get purchase data
                var purchaseItems = await _purchaseItemRepository.GetByMedicineAsync(medicine.Id);
                var purchasesInPeriod = purchaseItems.Where(pi => 
                    pi.Purchase?.Date >= startDate && pi.Purchase?.Date <= endDate).ToList();

                var totalPurchased = purchasesInPeriod.Sum(pi => pi.Qty);
                var totalCost = purchasesInPeriod.Sum(pi => pi.Subtotal);

                var profit = totalRevenue - totalCost;
                var profitMargin = totalRevenue > 0 ? (profit / totalRevenue) * 100 : 0;

                performanceReports.Add(new MedicinePerformanceReportDto
                {
                    MedicineId = medicine.Id,
                    MedicineName = medicine.Name,
                    MedicineBarcode = medicine.Barcode ?? "",
                    CategoryName = "", // Will be populated if needed
                    CurrentStock = medicine.Quantity,
                    TotalSold = totalSold,
                    TotalPurchased = totalPurchased,
                    TotalRevenue = totalRevenue,
                    TotalCost = totalCost,
                    Profit = profit,
                    ProfitMargin = profitMargin,
                    TurnoverRate = medicine.Quantity > 0 ? (double)totalSold / medicine.Quantity : 0
                });
            }

            return performanceReports.OrderByDescending(r => r.Profit);
        }
    }
}
