using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Repositories.Implementations
{
    public class CartItemRepository : BaseRepository<CartItem>, ICartItemRepository
    {
        private readonly AppDbContext _context;

        public CartItemRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<CartItem?> GetByIdAndCartAsync(Guid cartItemId, Guid cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.CartId == cartId);
        }

        public async Task<CartItem?> GetByCartAndProductAsync(Guid cartId, Guid productId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public void DeleteRange(IEnumerable<CartItem> entities)
        {
            _context.CartItems.RemoveRange(entities);
        }
    }
}
