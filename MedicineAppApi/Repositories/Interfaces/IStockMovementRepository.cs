using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface IStockMovementRepository : IRepository<StockMovement>
    {
        Task<IEnumerable<StockMovement>> GetByMedicineAsync(int medicineId);
        Task<IEnumerable<StockMovement>> GetByMedicineAndDateRangeAsync(int medicineId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<StockMovement>> GetByReferenceAsync(string referenceType, int referenceId);
        Task<IEnumerable<StockMovement>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<StockMovement>> GetByCreatedByAsync(int createdBy);
        Task<int> GetTotalInByMedicineAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null);
        Task<int> GetTotalOutByMedicineAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalValueByMedicineAsync(int medicineId, DateTime? startDate = null, DateTime? endDate = null);
        Task<IEnumerable<StockMovement>> GetLowStockMovementsAsync(int threshold);
        Task<DateTime?> GetLastMovementDateAsync(int medicineId);
    }
}
