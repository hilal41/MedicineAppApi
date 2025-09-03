using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByNameAsync(string name);
        Task<Customer?> GetByPhoneAsync(string phone);
        Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm);
        Task<IEnumerable<Customer>> GetTopCustomersAsync(int count = 10);
        Task<decimal> GetTotalSpentByCustomerAsync(int customerId);
        Task<int> GetInvoiceCountByCustomerAsync(int customerId);
    }
}
