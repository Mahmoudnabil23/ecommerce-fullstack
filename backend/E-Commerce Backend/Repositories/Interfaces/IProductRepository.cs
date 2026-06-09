using E_Commerce_Backend.DTOs.Products.Requests;
using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Required by: GET /products with all filters + pagination
        Task<(IQueryable<Product> Products, int Total)> GetPagedAsync(ProductFilterRequestDto filter);
        Task<int?> GetAvailableStockAsync(Guid productId);

        Task<bool> ExistsAsync(Guid productId);
        // Persist changes to the underlying DbContext


        // Required by: GET /admin/dashboard → topProducts
        Task<List<Product>> GetTopProductsAsync(int count);

        Task<int> SaveChangesAsync();




    }
}
