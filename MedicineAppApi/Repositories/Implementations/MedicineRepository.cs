using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class MedicineRepository : Repository<Medicine>, IMedicineRepository
    {
        public MedicineRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Medicine?> GetByNameAsync(string name)
        {
            return await _dbSet.Include(m => m.Category)
                               .FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<Medicine?> GetByBarcodeAsync(string barcode)
        {
            return await _dbSet.Include(m => m.Category)
                               .FirstOrDefaultAsync(m => m.Barcode == barcode);
        }

        public async Task<IEnumerable<Medicine>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet.Include(m => m.Category)
                               .Where(m => m.CategoryId == categoryId)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Medicine>> GetExpiringSoonAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _dbSet.Include(m => m.Category)
                               .Where(m => m.ExpiryDate <= thresholdDate && m.Quantity > 0)
                               .ToListAsync();
        }

        public async Task<bool> NameExistsAsync(string name)
        {
            return await _dbSet.AnyAsync(m => m.Name == name);
        }

        public async Task<bool> BarcodeExistsAsync(string barcode)
        {
            return await _dbSet.AnyAsync(m => m.Barcode == barcode);
        }

        public async Task<bool> BatchExistsAsync(string batch)
        {
            return await _dbSet.AnyAsync(m => m.Batch == batch);
        }
    }
}
