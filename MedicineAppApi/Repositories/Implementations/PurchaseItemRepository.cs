using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class PurchaseItemRepository : Repository<PurchaseItem>, IPurchaseItemRepository
    {
        public PurchaseItemRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PurchaseItem>> GetByPurchaseAsync(int purchaseId)
        {
            return await _dbSet.Include(pi => pi.Medicine)
                               .Where(pi => pi.PurchaseId == purchaseId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseItem>> GetByMedicineAsync(int medicineId)
        {
            return await _dbSet.Include(pi => pi.Purchase)
                               .Where(pi => pi.MedicineId == medicineId)
                               .OrderByDescending(pi => pi.Purchase.Date)
                               .ToListAsync();
        }

        public async Task<decimal> GetTotalPurchasesByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(pi => pi.MedicineId == medicineId && 
                                           pi.Purchase.Date >= startDate && 
                                           pi.Purchase.Date <= endDate)
                               .SumAsync(pi => pi.Subtotal);
        }

        public async Task<int> GetTotalQuantityPurchasedByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(pi => pi.MedicineId == medicineId && 
                                           pi.Purchase.Date >= startDate && 
                                           pi.Purchase.Date <= endDate)
                               .SumAsync(pi => pi.Qty);
        }
    }
}
