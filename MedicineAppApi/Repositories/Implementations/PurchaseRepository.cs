using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class PurchaseRepository : Repository<Purchase>, IPurchaseRepository
    {
        public PurchaseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Purchase?> GetByInvoiceNoAsync(string invoiceNo)
        {
            return await _dbSet.Include(p => p.Supplier)
                               .Include(p => p.CreatedByUser)
                               .Include(p => p.PurchaseItems)
                                   .ThenInclude(pi => pi.Medicine)
                               .FirstOrDefaultAsync(p => p.InvoiceNo == invoiceNo);
        }

        public async Task<IEnumerable<Purchase>> GetBySupplierAsync(int supplierId)
        {
            return await _dbSet.Include(p => p.Supplier)
                               .Include(p => p.CreatedByUser)
                               .Include(p => p.PurchaseItems)
                                   .ThenInclude(pi => pi.Medicine)
                               .Where(p => p.SupplierId == supplierId)
                               .OrderByDescending(p => p.Date)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Purchase>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Include(p => p.Supplier)
                               .Include(p => p.CreatedByUser)
                               .Include(p => p.PurchaseItems)
                                   .ThenInclude(pi => pi.Medicine)
                               .Where(p => p.Date >= startDate && p.Date <= endDate)
                               .OrderByDescending(p => p.Date)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Purchase>> GetByCreatedByAsync(int createdBy)
        {
            return await _dbSet.Include(p => p.Supplier)
                               .Include(p => p.CreatedByUser)
                               .Include(p => p.PurchaseItems)
                                   .ThenInclude(pi => pi.Medicine)
                               .Where(p => p.CreatedBy == createdBy)
                               .OrderByDescending(p => p.Date)
                               .ToListAsync();
        }

        public async Task<bool> InvoiceNoExistsAsync(string invoiceNo)
        {
            return await _dbSet.AnyAsync(p => p.InvoiceNo == invoiceNo);
        }

        public async Task<decimal> GetTotalPurchasesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(p => p.Date >= startDate && p.Date <= endDate)
                               .SumAsync(p => p.Total);
        }

        public async Task<int> GetPurchaseCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(p => p.Date >= startDate && p.Date <= endDate)
                               .CountAsync();
        }
    }
}
