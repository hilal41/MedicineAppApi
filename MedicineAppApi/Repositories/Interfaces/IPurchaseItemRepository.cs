using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface IPurchaseItemRepository : IRepository<PurchaseItem>
    {
        Task<IEnumerable<PurchaseItem>> GetByPurchaseAsync(int purchaseId);
        Task<IEnumerable<PurchaseItem>> GetByMedicineAsync(int medicineId);
        Task<decimal> GetTotalPurchasesByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate);
        Task<int> GetTotalQuantityPurchasedByMedicineAsync(int medicineId, DateTime startDate, DateTime endDate);
    }
}
