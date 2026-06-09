using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        // Required by: GET /cart for authenticated user
        Task<Cart?> GetByUserIdAsync(Guid userId);

        // Required by: GET /cart for guest user (X-Guest-Session header)
        Task<Cart?> GetByGuestSessionAsync(string guestSessionId);

        // Required by: POST /cart/merge — load guest cart with items
        Task<Cart?> GetByGuestSessionWithItemsAsync(string guestSessionId);
    }
}
