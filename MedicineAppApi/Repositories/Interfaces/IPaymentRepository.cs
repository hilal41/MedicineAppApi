using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetByInvoiceAsync(int invoiceId);
        Task<IEnumerable<Payment>> GetByMethodAsync(string method);
        Task<IEnumerable<Payment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Payment>> GetByReceivedByAsync(int receivedBy);
        Task<decimal> GetTotalPaymentsByInvoiceAsync(int invoiceId);
        Task<decimal> GetTotalPaymentsByMethodAsync(string method, DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> GetPaymentCountByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
