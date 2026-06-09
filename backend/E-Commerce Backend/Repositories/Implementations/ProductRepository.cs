using E_Commerce_Backend.DTOs.Products.Requests;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Repositories.Implementations
{
    public class ProductRepository(AppDbContext context) : BaseRepository<Product>(context), IProductRepository
    {
        public async Task<bool> ExistsAsync(Guid productId)
        {
            return await context.Products.AnyAsync(p => p.Id == productId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<int?> GetAvailableStockAsync(Guid productId)
        {
            return (await context.Products
                .FirstOrDefaultAsync(p => p.Id == productId))?.Stock;
        }

        public async Task<(IQueryable<Product> Products, int Total)> GetPagedAsync(ProductFilterRequestDto filter)
        {
            int total = await context.Products.CountAsync();

            IQueryable<Product> query = context.Products.AsQueryable();

            if(filter.MinPrice is not null)
                query = query.Where(p => p.Price >= filter.MinPrice);

            if(filter.MaxPrice is not null)
                query = query.Where(p => p.Price <= filter.MaxPrice);

            switch(filter.SortBy)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.Price);
                    break;

                case "price_desc":
                    query = query.OrderByDescending(p => p.Price);
                    break;

                case "newest":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;

                case "rating":
                    query = query.Select(p => new
                    {
                        Product = p,
                        TotalRating = context.Reviews
                        .Where(r => r.ProductId == p.Id)
                        .Sum(r => r.Rating)
                    }).OrderByDescending(p => p.TotalRating)
                    .Select(p => p.Product);
                    break;

                default:
                    query = query.OrderBy(p => p.Name);
                    break;
            }

            if (filter.Search is not null)
                query = query
                    .Where(p => p.Name.ToLower().Contains(filter.Search.ToLower()));

            query = query.Skip(filter.Limit * (filter.Page - 1)).Take(filter.Limit);

            return (query, total);
        }

        public async Task<List<Product>> GetTopProductsAsync(int count)
        {
            return await context.Products
                .Select(p => new
                {
                    Product = p,
                    TotalRating = context.Reviews
                        .Where(r => r.ProductId == p.Id)
                        .Sum(r => r.Rating)
                })
                .OrderByDescending(p => p.TotalRating)
                .Select(p => p.Product)
                .Take(count)
                .ToListAsync();
        }
    }
}
