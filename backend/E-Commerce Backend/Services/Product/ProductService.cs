using E_Commerce_Backend.DTOs.Products.Requests;
using E_Commerce_Backend.DTOs.Products.Responses;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Services.Products
{
    public class ProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepository)
        {
            _productRepo = productRepository;
        }

        public async Task<List<ProductListItemDto>> GetProductsListAsync(ProductFilterRequestDto filter)
        {
            (IQueryable<Product>, int) pagedProducts = await _productRepo.GetPagedAsync(filter);

            List<ProductListItemDto> productsList = await pagedProducts.Item1
                .Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                DiscountedPrice = p.DiscountedPrice,
                Images = p.Images.Select(i => i.ImageUrl).ToList(),
                Stock = p.Stock,
                AverageRating = p.Reviews.Any() ?  p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews.Count()
            }).ToListAsync();

            return productsList;
        }

        public async Task<int?> GetAvailableStockAsync(Guid productId)
        {
            return await _productRepo.GetAvailableStockAsync(productId);
        }

        public async Task<bool> ExistsAsync(Guid productId)
        {
            return await _productRepo.ExistsAsync(productId);
        }

        public async Task AddNewAsync(CreateProductRequestDto dto)
        {
            Product product = new()
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                SpecsJson = dto.SpecsJson
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();
        }

        public async Task<ProductDetailResponseDto?> GetByIdAsync(Guid id)
        {
            var product = await _productRepo.GetAll()
                .Where(p => p.Id == id)
                .Select(p =>
                    new ProductDetailResponseDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Slug = p.Slug ?? string.Empty,
                        Price = p.Price,
                        DiscountedPrice = p.DiscountedPrice,
                        Images = p.Images.Select(i => i.ImageUrl).ToList(),
                        Stock = p.Stock,
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
                        ReviewCount = p.Reviews.Count(),
                        Description = p.Description,
                        SpecsJson = p.SpecsJson,
                        CreatedAt = p.CreatedAt
                    }).FirstOrDefaultAsync();

            return product;
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateProductRequestDto dto)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product is null)
                return false;

            if (dto.Name is not null) product.Name = dto.Name;
            if (dto.Description is not null) product.Description = dto.Description;
            if (dto.Price is not null) product.Price = dto.Price.Value;
            if (dto.Stock is not null) product.Stock = dto.Stock.Value;
            if (dto.SpecsJson is not null) product.SpecsJson = dto.SpecsJson;

            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product is null)
                return false;

            // Soft delete
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;

            _productRepo.Update(product);
            await _productRepo.SaveChangesAsync();

            return true;
        }
    }
}
