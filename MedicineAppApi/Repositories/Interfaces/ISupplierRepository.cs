using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface ISupplierRepository : IRepository<Supplier>
    {
        Task<Supplier?> GetByNameAsync(string name);
        Task<bool> NameExistsAsync(string name);
    }
}
