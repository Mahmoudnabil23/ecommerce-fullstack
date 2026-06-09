using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface ISellerProfileRepository : IGenericRepository<SellerProfile>
    {
        // Required by: GET /sellers/me — resolved from authenticated user
        Task<SellerProfile?> GetByUserIdAsync(Guid userId);

        // Required by: GET /admin/sellers — pagination with status filter
        Task<(List<SellerProfile> Sellers, int Total)> GetPagedAsync(
            SellerStatus? status,
            int page,
            int limit);
    }
}
