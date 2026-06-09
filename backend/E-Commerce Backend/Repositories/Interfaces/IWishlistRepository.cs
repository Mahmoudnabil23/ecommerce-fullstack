using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IWishlistRepository : IGenericRepository<WishlistItem>
    {
        // Required by: GET /wishlist
        Task<List<WishlistItem>> GetByUserIdAsync(Guid userId);

        // Required by: DELETE /wishlist/{productId} and move-to-cart
        Task<WishlistItem?> GetByUserAndProductAsync(Guid userId, Guid productId);

        // Required by: POST /wishlist → 409 duplicate check
        Task<bool> ExistsAsync(Guid userId, Guid productId);
    }
}
