using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface IMedicineRepository : IRepository<Medicine>
    {
        Task<Medicine?> GetByNameAsync(string name);
        Task<Medicine?> GetByBarcodeAsync(string barcode);
        Task<IEnumerable<Medicine>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Medicine>> GetExpiringSoonAsync(int daysThreshold);
        Task<bool> NameExistsAsync(string name);
        Task<bool> BarcodeExistsAsync(string barcode);
        Task<bool> BatchExistsAsync(string batch);
    }
}
