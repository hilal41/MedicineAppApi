using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<Invoice?> GetByInvoiceNoAsync(string invoiceNo);
        Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Invoice>> GetByCustomerAsync(int customerId);
        Task<IEnumerable<Invoice>> GetByCreatedByAsync(int createdBy);
        Task<bool> InvoiceNoExistsAsync(string invoiceNo);
        Task<decimal> GetTotalSalesAsync(DateTime startDate, DateTime endDate);
        Task<int> GetInvoiceCountAsync(DateTime startDate, DateTime endDate);
    }
}
