using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class InvoiceItemRepository : Repository<InvoiceItem>, IInvoiceItemRepository
    {
        public InvoiceItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InvoiceItem>> GetByInvoiceAsync(int invoiceId)
        {
            return await _dbSet.Include(ii => ii.Medicine)
                               .Where(ii => ii.InvoiceId == invoiceId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceItem>> GetByMedicineAsync(int medicineId)
        {
            return await _dbSet.Include(ii => ii.Invoice)
                               .Where(ii => ii.MedicineId == medicineId)
                               .OrderByDescending(ii => ii.Invoice.Date)
                               .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(ii => ii.MedicineId == medicineId && 
                                           ii.Invoice.Date >= startDate && 
                                           ii.Invoice.Date <= endDate)
                               .SumAsync(ii => ii.Subtotal);
        }

        public async Task<int> GetTotalQuantitySoldByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(ii => ii.MedicineId == medicineId && 
                                           ii.Invoice.Date >= startDate && 
                                           ii.Invoice.Date <= endDate)
                               .SumAsync(ii => ii.Qty);
        }
    }
}
