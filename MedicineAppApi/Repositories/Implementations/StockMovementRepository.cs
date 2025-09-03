using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class StockMovementRepository : Repository<StockMovement>, IStockMovementRepository
    {
        public StockMovementRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StockMovement>> GetByMedicineAsync(int medicineId)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.CreatedByUser)
                               .Where(sm => sm.MedicineId == medicineId)
                               .OrderByDescending(sm => sm.CreatedAt)
                               .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetByMedicineAndDateRangeAsync(int medicineId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.CreatedByUser)
                               .Where(sm => sm.MedicineId == medicineId && 
                                          sm.CreatedAt >= startDate && 
                                          sm.CreatedAt <= endDate)
                               .OrderByDescending(sm => sm.CreatedAt)
                               .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetByReferenceAsync(string referenceType, int referenceId)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.CreatedByUser)
                               .Where(sm => sm.ReferenceType == referenceType && 
                                          sm.ReferenceId == referenceId)
                               .OrderByDescending(sm => sm.CreatedAt)
                               .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.CreatedByUser)
                               .Where(sm => sm.CreatedAt >= startDate && 
                                          sm.CreatedAt <= endDate)
                               .OrderByDescending(sm => sm.CreatedAt)
                               .ToListAsync();
        }

        public async Task<IEnumerable<StockMovement>> GetByCreatedByAsync(int createdBy)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.CreatedByUser)
                               .Where(sm => sm.CreatedBy == createdBy)
                               .OrderByDescending(sm => sm.CreatedAt)
                               .ToListAsync();
        }

        public async Task<int> GetTotalInByMedicineAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(sm => sm.MedicineId == medicineId && sm.ChangeQty > 0);
            
            if (startDate.HasValue)
                query = query.Where(sm => sm.CreatedAt >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(sm => sm.CreatedAt <= endDate.Value);
            
            return await query.SumAsync(sm => sm.ChangeQty);
        }

        public async Task<int> GetTotalOutByMedicineAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(sm => sm.MedicineId == medicineId && sm.ChangeQty < 0);
            
            if (startDate.HasValue)
                query = query.Where(sm => sm.CreatedAt >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(sm => sm.CreatedAt <= endDate.Value);
            
            return Math.Abs(await query.SumAsync(sm => sm.ChangeQty));
        }

        public async Task<decimal> GetTotalValueByMedicineAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Include(sm => sm.Medicine)
                              .Where(sm => sm.MedicineId == medicineId);
            
            if (startDate.HasValue)
                query = query.Where(sm => sm.CreatedAt >= startDate.Value);
            
            if (endDate.HasValue)
                query = query.Where(sm => sm.CreatedAt <= endDate.Value);
            
            return await query.SumAsync(sm => Math.Abs(sm.ChangeQty) * sm.Medicine.Price);
        }

        public async Task<IEnumerable<StockMovement>> GetLowStockMovementsAsync(int threshold)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.CreatedByUser)
                               .Where(sm => sm.Medicine.Quantity <= threshold)
                               .OrderByDescending(sm => sm.CreatedAt)
                               .ToListAsync();
        }

        public async Task<DateTime?> GetLastMovementDateAsync(int medicineId)
        {
            var lastMovement = await _dbSet.Where(sm => sm.MedicineId == medicineId)
                                           .OrderByDescending(sm => sm.CreatedAt)
                                           .FirstOrDefaultAsync();
            
            return lastMovement?.CreatedAt;
        }
    }
}
