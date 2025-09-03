using Microsoft.EntityFrameworkCore;
using MedicineAppApi.Data;
using MedicineAppApi.Models;
using MedicineAppApi.Repositories.Interfaces;
using MedicineAppApi.Repositories.Implementations;

namespace MedicineAppApi.Repositories.Implementations
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Customer?> GetByPhoneAsync(string phone)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Phone == phone);
        }

        public async Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm)
        {
            return await _dbSet.Where(c => c.Name.Contains(searchTerm))
                               .OrderBy(c => c.Name)
                               .ToListAsync();
        }

        public async Task<IEnumerable<Customer>> GetTopCustomersAsync(int count = 10)
        {
            return await _dbSet.Include(c => c.Invoices)
                               .OrderByDescending(c => c.Invoices.Sum(i => i.Total))
                               .Take(count)
                               .ToListAsync();
        }

        public async Task<decimal> GetTotalSpentByCustomerAsync(int customerId)
        {
            return await _dbSet.Where(c => c.Id == customerId)
                               .SelectMany(c => c.Invoices)
                               .SumAsync(i => i.Total);
        }

        public async Task<int> GetInvoiceCountByCustomerAsync(int customerId)
        {
            return await _dbSet.Where(c => c.Id == customerId)
                               .SelectMany(c => c.Invoices)
                               .CountAsync();
        }
    }
}
