using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Admin.Requests;
using E_Commerce_Backend.DTOs.Products.Requests;
using E_Commerce_Backend.DTOs.Products.Responses;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories;
using E_Commerce_Backend.Services.AdminServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Services.AdminServices
{
    public class AdminProductService : IAdminProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductDetailResponseDto>> AddAsync(CreateProductRequestDto productRequestDto)
        {
            Product product = new()
            {
                Name = productRequestDto.Name,
                Description = productRequestDto.Description,
                Price = productRequestDto.Price,
                CategoryId = productRequestDto.CategoryId,
                Stock = productRequestDto.Stock,
                SpecsJson = productRequestDto.SpecsJson
                // Note: If admins are assigning a Seller to the product, ensure SellerId is mapped here if it exists in your DTO.
            };

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            ProductDetailResponseDto productResponse = await GetProductDetailsDto(product);
            return Result<ProductDetailResponseDto>.Ok(productResponse);
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            Product? product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return Result<bool>.Fail($"Product with id: {id} was not found");
            }
            product.IsDeleted = true;
            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Product Deleted Successfully");
        }

        public async Task<Result<object>> GetAllAsync(AdminProductFilterRequestDto request)
        {
            IQueryable<Product> query = _unitOfWork.Products.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Search))
                query = query.Where(p => p.Name.Contains(request.Search) || p.Slug.Contains(request.Search));

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.SellerId.HasValue)
                query = query.Where(p => p.SellerId == request.SellerId.Value);

            if (request.MinPrice.HasValue)
                query = query.Where(p => p.Price >= request.MinPrice.Value);

            if (request.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= request.MaxPrice.Value);

            int totalCount = await query.CountAsync();

            List<ProductListItemDto> products = await query
                .OrderByDescending(p => p.Id)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .Select(product => new ProductListItemDto()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Images = product.Images.Select(i => i.ImageUrl).ToList(),
                    Category = new CategoryRefDto()
                    {
                        Id = product.CategoryId.Value,
                        Name = product.Category.Name
                    },
                    Seller = new SellerRefDto()
                    {
                        Id = product.SellerId.Value,
                        StoreName = product.Seller.StoreName
                    },
                    Slug = product.Slug,
                    Stock = product.Stock,
                    DiscountedPrice = product.DiscountedPrice
                })
                .ToListAsync();


            foreach (ProductListItemDto dto in products)
            {
                (double average, int count) review = await _unitOfWork.Reviews.GetRatingSummaryAsync(dto.Id);

                dto.ReviewCount = review.count;
                dto.AverageRating = review.average;
            }

            var pagedData = new
            {
                TotalItems = totalCount,
                CurrentPage = request.Page,
                PageSize = request.Limit,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.Limit),
                Products = products
            };

            return Result<object>.Ok(pagedData);
        }

        public async Task<Result<ProductDetailResponseDto>> GetByIdAsync(Guid id)
        {
            Product? product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return Result<ProductDetailResponseDto>.Fail($"Product with id: {id} was not found");
            }

            ProductDetailResponseDto productDetailResponse = await GetProductDetailsDto(product);

            return Result<ProductDetailResponseDto>.Ok(productDetailResponse);
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateProductRequestDto productRequestDto)
        {
            Product? product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return Result<bool>.Fail($"Product with id: {id} was not found");
            }

            if (!string.IsNullOrWhiteSpace(productRequestDto.Name))
            {
                product.Name = productRequestDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(productRequestDto.Description))
            {
                product.Description = productRequestDto.Description;
            }

            if (productRequestDto.Price.HasValue)
            {
                product.Price = productRequestDto.Price.Value;
            }

            if (productRequestDto.CategoryId.HasValue)
            {
                product.CategoryId = productRequestDto.CategoryId.Value;
            }

            if (productRequestDto.Stock.HasValue)
            {
                product.Stock = productRequestDto.Stock.Value;
            }

            if (productRequestDto.SpecsJson != null)
            {
                product.SpecsJson = productRequestDto.SpecsJson;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Product Updated Successfully");
        }

        private async Task<ProductDetailResponseDto> GetProductDetailsDto(Product product)
        {
            (double average, int count) review = await _unitOfWork.Reviews.GetRatingSummaryAsync(product.Id);

            ProductDetailResponseDto productDetailResponse = new ProductDetailResponseDto()
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Images = product.Images.Select(i => i.ImageUrl).ToList(),
                Category = new CategoryRefDto()
                {
                    Id = product.CategoryId.Value,
                    Name = product.Category.Name // Assumes eager loading or navigation property exists
                },
                Seller = new SellerRefDto()
                {
                    Id = product.SellerId.Value,
                    StoreName = product.Seller.StoreName // Assumes eager loading or navigation property exists
                },
                Slug = product.Slug,
                Stock = product.Stock,
                ReviewCount = review.count,
                AverageRating = review.average,
                DiscountedPrice = product.DiscountedPrice,
                CreatedAt = product.CreatedAt,
                Description = product.Description,
                SpecsJson = product.SpecsJson
            };

            return productDetailResponse;
        }
    }
}