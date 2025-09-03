using MedicineAppApi.Models;

namespace MedicineAppApi.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<bool> NameExistsAsync(string name);
    }
}
