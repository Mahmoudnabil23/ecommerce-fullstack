using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Admin.Requests;
using E_Commerce_Backend.DTOs.Products.Requests;
using E_Commerce_Backend.DTOs.Products.Responses;
using E_Commerce_Backend.Services.AdminServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Backend.Controllers.Admin
{
    [Route("api/admin/products")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminProductController : ControllerBase
    {
        private readonly IAdminProductService _productService;

        public AdminProductController(IAdminProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] AdminProductFilterRequestDto request)
        {
            Result<object> result = await _productService.GetAllAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Products retrieved successfully",
                Data = result.Data
            });
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ApiResponse<ProductDetailResponseDto>>> GetById(Guid id)
        {
            Result<ProductDetailResponseDto> result = await _productService.GetByIdAsync(id);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse<ProductDetailResponseDto>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return Ok(new ApiResponse<ProductDetailResponseDto>()
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDetailResponseDto>>> Add(CreateProductRequestDto productRequestDto)
        {
            Result<ProductDetailResponseDto> result = await _productService.AddAsync(productRequestDto);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse<ProductDetailResponseDto>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new ApiResponse<ProductDetailResponseDto>
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<ApiResponse>> SoftDelete(Guid id)
        {
            Result<bool> result = await _productService.SoftDeleteAsync(id);

            if (!result.Success)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return Ok(new ApiResponse()
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpPut("{id:Guid}")]
        public async Task<ActionResult<ApiResponse>> Update(Guid id,UpdateProductRequestDto productRequestDto)
        {
            Result<bool> result = await _productService.UpdateAsync(id, productRequestDto);

            if (!result.Success)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }
            return Ok(new ApiResponse()
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }
    }
}
