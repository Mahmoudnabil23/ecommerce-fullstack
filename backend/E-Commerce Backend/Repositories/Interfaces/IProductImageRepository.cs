using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IProductImageRepository : IGenericRepository<ProductImage>
    {
        // Required by: DELETE /products/{id}/images/{imageId} ownership check
        Task<ProductImage?> GetByIdAndProductAsync(Guid imageId, Guid productId);
    }
}
