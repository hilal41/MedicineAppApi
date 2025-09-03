using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface ISupplierMedicineRepository : IRepository<SupplierMedicine>
    {
        Task<IEnumerable<SupplierMedicine>> GetBySupplierAsync(int supplierId);
        Task<IEnumerable<SupplierMedicine>> GetByMedicineAsync(int medicineId);
        Task<SupplierMedicine?> GetBySupplierAndMedicineAsync(int supplierId, int medicineId);
        Task<bool> ExistsAsync(int supplierId, int medicineId);
    }
}
