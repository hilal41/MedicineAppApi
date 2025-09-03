using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface IPurchaseRepository : IRepository<Purchase>
    {
        Task<Purchase?> GetByInvoiceNoAsync(string invoiceNo);
        Task<IEnumerable<Purchase>> GetBySupplierAsync(int supplierId);
        Task<IEnumerable<Purchase>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Purchase>> GetByCreatedByAsync(int createdBy);
        Task<bool> InvoiceNoExistsAsync(string invoiceNo);
        Task<decimal> GetTotalPurchasesAsync(DateTime startDate, DateTime endDate);
        Task<int> GetPurchaseCountAsync(DateTime startDate, DateTime endDate);
    }
}
