using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface IInvoiceItemRepository : IRepository<InvoiceItem>
    {
        Task<IEnumerable<InvoiceItem>> GetByInvoiceAsync(int invoiceId);
        Task<IEnumerable<InvoiceItem>> GetByMedicineAsync(int medicineId);
        Task<decimal> GetTotalSalesByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate);
        Task<int> GetTotalQuantitySoldByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate);
    }
}
