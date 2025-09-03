using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class SupplierMedicineRepository : Repository<SupplierMedicine>, ISupplierMedicineRepository
    {
        public SupplierMedicineRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SupplierMedicine>> GetBySupplierAsync(int supplierId)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.Supplier)
                               .Where(sm => sm.SupplierId == supplierId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<SupplierMedicine>> GetByMedicineAsync(int medicineId)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.Supplier)
                               .Where(sm => sm.MedicineId == medicineId)
                               .ToListAsync();
        }

        public async Task<SupplierMedicine?> GetBySupplierAndMedicineAsync(int supplierId, int medicineId)
        {
            return await _dbSet.Include(sm => sm.Medicine)
                               .Include(sm => sm.Supplier)
                               .FirstOrDefaultAsync(sm => sm.SupplierId == supplierId && sm.MedicineId == medicineId);
        }

        public async Task<bool> ExistsAsync(int supplierId, int medicineId)
        {
            return await _dbSet.AnyAsync(sm => sm.SupplierId == supplierId && sm.MedicineId == medicineId);
        }
    }
}
