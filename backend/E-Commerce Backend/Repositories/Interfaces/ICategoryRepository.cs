using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Required by: GET /categories → full tree structure
        Task<List<Category>> GetTreeAsync();

        // Required by: POST /categories slug uniqueness check
        Task<bool> SlugExistsAsync(string slug);
    }
}
