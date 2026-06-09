using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface ICartItemRepository : IGenericRepository<CartItem>
    {
        // Required by: PUT /cart/items/{cartItemId} — validate ownership
        Task<CartItem?> GetByIdAndCartAsync(Guid cartItemId, Guid cartId);

        // Required by: POST /cart/items — check if product already in cart
        Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId);
        void DeleteRange(IEnumerable<CartItem> entities);

    }
}
