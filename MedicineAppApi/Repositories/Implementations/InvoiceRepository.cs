using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Invoice?> GetByInvoiceNoAsync(string invoiceNo)
        {
            return await _dbSet.Include(i => i.CreatedByUser)
                               .Include(i => i.InvoiceItems)
                                   .ThenInclude(ii => ii.Medicine)
                               .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo);
        }

        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Include(i => i.CreatedByUser)
                               .Include(i => i.InvoiceItems)
                                   .ThenInclude(ii => ii.Medicine)
                               .Where(i => i.Date >= startDate && i.Date <= endDate)
                               .OrderByDescending(i => i.Date)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId)
        {
            return await _dbSet.Include(i => i.CreatedByUser)
                               .Include(i => i.Customer)
                               .Include(i => i.InvoiceItems)
                                   .ThenInclude(ii => ii.Medicine)
                               .Where(i => i.CustomerId == customerId)
                               .OrderByDescending(i => i.Date)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByCreatedByAsync(int createdBy)
        {
            return await _dbSet.Include(i => i.CreatedByUser)
                               .Include(i => i.InvoiceItems)
                                   .ThenInclude(ii => ii.Medicine)
                               .Where(i => i.CreatedBy == createdBy)
                               .OrderByDescending(i => i.Date)
                               .ToListAsync();
        }

        public async Task<bool> InvoiceNoExistsAsync(string invoiceNo)
        {
            return await _dbSet.AnyAsync(i => i.InvoiceNo == invoiceNo);
        }

        public async Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.Date >= startDate && i.Date <= endDate)
                               .SumAsync(i => i.Total);
        }

        public async Task<int> GetInvoiceCountAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(i => i.Date >= startDate && i.Date <= endDate)
                               .CountAsync();
        }
    }
}
