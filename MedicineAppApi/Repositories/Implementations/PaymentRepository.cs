using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetByInvoiceAsync(int invoiceId)
        {
            return await _dbSet.Include(p => p.Invoice)
                               .Include(p => p.ReceivedByUser)
                               .Where(p => p.InvoiceId == invoiceId)
                               .OrderByDescending(p => p.Date)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByMethodAsync(string method)
        {
            return await _dbSet.Include(p => p.Invoice)
                               .Include(p => p.ReceivedByUser)
                               .Where(p => p.Method == method)
                               .OrderByDescending(p => p.Date)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Include(p => p.Invoice)
                               .Include(p => p.ReceivedByUser)
                               .Where(p => p.Date >= startDate && p.Date <= endDate)
                               .OrderByDescending(p => p.Date)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByReceivedByAsync(int receivedBy)
        {
            return await _dbSet.Include(p => p.Invoice)
                               .Include(p => p.ReceivedByUser)
                               .Where(p => p.ReceivedBy == receivedBy)
                               .OrderByDescending(p => p.Date)
                               .ToListAsync();
        }

        public async Task<decimal> GetTotalPaymentsByInvoiceAsync(int invoiceId)
        {
            return await _dbSet.Where(p => p.InvoiceId == invoiceId)
                               .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetTotalPaymentsByMethodAsync(string method, DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(p => p.Method == method && 
                                          p.Date >= startDate && 
                                          p.Date <= endDate)
                               .SumAsync(p => p.Amount);
        }

        public async Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(p => p.Date >= startDate && p.Date <= endDate)
                               .SumAsync(p => p.Amount);
        }

        public async Task<int> GetPaymentCountByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet.Where(p => p.Date >= startDate && p.Date <= endDate)
                               .CountAsync();
        }
    }
}
