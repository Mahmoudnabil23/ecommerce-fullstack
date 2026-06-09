using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Categories.Responses;
using E_Commerce_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CategoryResponseDto>>>> GetCategories()
        {
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync();

            var lookup = categories.ToDictionary(
                c => c.Id,
                c => new CategoryResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    ImageUrl = c.ImageUrl
                });

            var roots = new List<CategoryResponseDto>();
            foreach (var c in categories)
            {
                var dto = lookup[c.Id];
                if (c.ParentId.HasValue && lookup.ContainsKey(c.ParentId.Value))
                {
                    lookup[c.ParentId.Value].Children.Add(dto);
                }
                else
                {
                    roots.Add(dto);
                }
            }

            return Ok(new ApiResponse<List<CategoryResponseDto>>
            {
                Success = true,
                Data = roots
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryResponseDto>>> GetCategory(Guid id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (category == null || category.IsDeleted)
            {
                return NotFound(new ApiResponse<CategoryResponseDto>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            var categoryDto = new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                ImageUrl = category.ImageUrl
            };

            return Ok(new ApiResponse<CategoryResponseDto>
            {
                Success = true,
                Data = categoryDto
            });
        }
    }
}