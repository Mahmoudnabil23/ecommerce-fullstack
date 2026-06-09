using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Repositories.Implementations
{
    public class ProductImageRepository(AppDbContext context) : BaseRepository<ProductImage>(context), IProductImageRepository
    {
        public async Task<ProductImage?> GetByIdAndProductAsync(Guid imageId, Guid productId)
        {
            return await context.ProductImages
                .Where(pi => pi.Id == imageId && pi.ProductId == productId)
                .FirstOrDefaultAsync();
        }
    }
}
