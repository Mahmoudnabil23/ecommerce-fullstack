using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Repositories.Implementations
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(Guid userId)
        {
            // UserId stored as string in Cart model (nullable); include related items
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
                .FirstOrDefaultAsync(c => c.UserId == userId.ToString());
        }

        public async Task<Cart?> GetByGuestSessionAsync(string guestSessionId)
        {
            return await _context.Carts
                .Include (c => c.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude (p => p.Images)
                .FirstOrDefaultAsync(c => c.GuestSessionId == guestSessionId);
        }

        public async Task<Cart?> GetByGuestSessionWithItemsAsync(string guestSessionId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.GuestSessionId == guestSessionId);
        }
    }
}
