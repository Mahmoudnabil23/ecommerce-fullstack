using E_Commerce_Backend.Comman;
using E_Commerce_Backend.Comman.Application.Common;
using E_Commerce_Backend.DTOs.Products.Requests;
using E_Commerce_Backend.DTOs.Products.Responses;
using E_Commerce_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ProductListItemDto>>>> GetProducts([FromQuery] ProductFilterRequestDto filter)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p => p.Name.Contains(filter.Search) || p.Description.Contains(filter.Search));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            if (filter.SellerId.HasValue)
            {
                query = query.Where(p => p.SellerId == filter.SellerId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            if (filter.InStock.HasValue)
            {
                query = filter.InStock.Value ? query.Where(p => p.Stock > 0) : query.Where(p => p.Stock <= 0);
            }

            query = filter.SortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "rating" => query.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            var total = await query.CountAsync();

            var page = filter.Page < 1 ? 1 : filter.Page;
            var limit = filter.Limit < 1 ? 20 : filter.Limit;

            var products = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            var items = products.Select(p => new ProductListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Price = p.Price,
                DiscountedPrice = p.DiscountedPrice,
                Images = p.Images.Select(i => i.ImageUrl).ToList(),
                Category = p.CategoryId.HasValue && p.Category != null
                    ? new CategoryRefDto { Id = p.CategoryId.Value, Name = p.Category.Name }
                    : new CategoryRefDto { Id = Guid.Empty, Name = "Uncategorized" },
                Seller = p.SellerId.HasValue && p.Seller != null
                    ? new SellerRefDto { Id = p.SellerId.Value, StoreName = p.Seller.StoreName }
                    : new SellerRefDto { Id = Guid.Empty, StoreName = "Unknown seller" },
                Stock = p.Stock,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => (double)r.Rating) : 0,
                ReviewCount = p.Reviews.Count
            }).ToList();

            var response = new PaginatedResponse<ProductListItemDto>
            {
                Items = items,
                Page = page,
                Limit = limit,
                Total = total
            };

            return Ok(new ApiResponse<PaginatedResponse<ProductListItemDto>>
            {
                Success = true,
                Data = response
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDetailResponseDto>>> GetProduct(Guid id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .Include(p => p.Images)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null || product.IsDeleted)
            {
                return NotFound(new ApiResponse<ProductDetailResponseDto>
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            var productDto = new ProductDetailResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                DiscountedPrice = product.DiscountedPrice,
                Images = product.Images.Select(i => i.ImageUrl).ToList(),
                Category = product.CategoryId.HasValue && product.Category != null
                    ? new CategoryRefDto { Id = product.CategoryId.Value, Name = product.Category.Name }
                    : new CategoryRefDto { Id = Guid.Empty, Name = "Uncategorized" },
                Seller = product.SellerId.HasValue && product.Seller != null
                    ? new SellerRefDto { Id = product.SellerId.Value, StoreName = product.Seller.StoreName }
                    : new SellerRefDto { Id = Guid.Empty, StoreName = "Unknown seller" },
                Stock = product.Stock,
                AverageRating = product.Reviews.Any() ? product.Reviews.Average(r => (double)r.Rating) : 0,
                ReviewCount = product.Reviews.Count,
                SpecsJson = product.SpecsJson,
                CreatedAt = product.CreatedAt
            };

            return Ok(new ApiResponse<ProductDetailResponseDto>
            {
                Success = true,
                Data = productDto
            });
        }

        [Authorize(Roles = "Seller,Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDetailResponseDto>>> CreateProduct([FromBody] CreateProductRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<ProductDetailResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted);
            if (!categoryExists)
            {
                return BadRequest(new ApiResponse<ProductDetailResponseDto>
                {
                    Success = false,
                    Message = "Invalid category"
                });
            }

            var seller = await _context.SellerProfiles.FirstOrDefaultAsync(s => s.UserId == userId);
            if (seller == null)
            {
                return StatusCode(403, new ApiResponse<ProductDetailResponseDto>
                {
                    Success = false,
                    Message = "Seller profile not found for current user"
                });
            }

            var baseSlug = GenerateSlug(request.Name);
            var slug = baseSlug;
            var suffix = 1;
            while (await _context.Products.AnyAsync(p => p.Slug == slug))
            {
                slug = $"{baseSlug}-{suffix}";
                suffix++;
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                SpecsJson = request.SpecsJson,
                Slug = slug,
                CategoryId = request.CategoryId,
                SellerId = seller.Id,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var createdProduct = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Seller)
                .FirstAsync(p => p.Id == product.Id);

            var productDto = new ProductDetailResponseDto
            {
                Id = createdProduct.Id,
                Name = createdProduct.Name,
                Slug = createdProduct.Slug,
                Description = createdProduct.Description,
                Price = createdProduct.Price,
                DiscountedPrice = createdProduct.DiscountedPrice,
                Images = new List<string>(),
                Category = createdProduct.CategoryId.HasValue && createdProduct.Category != null
                    ? new CategoryRefDto { Id = createdProduct.CategoryId.Value, Name = createdProduct.Category.Name }
                    : new CategoryRefDto { Id = Guid.Empty, Name = "Uncategorized" },
                Seller = createdProduct.SellerId.HasValue && createdProduct.Seller != null
                    ? new SellerRefDto { Id = createdProduct.SellerId.Value, StoreName = createdProduct.Seller.StoreName }
                    : new SellerRefDto { Id = Guid.Empty, StoreName = "Unknown seller" },
                Stock = createdProduct.Stock,
                AverageRating = 0,
                ReviewCount = 0,
                SpecsJson = createdProduct.SpecsJson,
                CreatedAt = createdProduct.CreatedAt
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new ApiResponse<ProductDetailResponseDto>
            {
                Success = true,
                Data = productDto
            });
        }

        private string GenerateSlug(string name)
        {
            return name.ToLower().Replace(" ", "-").Replace("/", "-");
        }
    }
}