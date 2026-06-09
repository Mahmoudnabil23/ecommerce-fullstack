using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IBannerRepository : IGenericRepository<Banner>
    {
        // Required by: Home page — only active banners, ordered by position
        Task<List<Banner>> GetActiveOrderedAsync();
    }
}
