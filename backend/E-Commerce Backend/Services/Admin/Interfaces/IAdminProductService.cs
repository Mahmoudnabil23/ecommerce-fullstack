using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Admin.Requests;
using E_Commerce_Backend.DTOs.Products.Requests;
using E_Commerce_Backend.DTOs.Products.Responses;

namespace E_Commerce_Backend.Services.AdminServices.Interfaces
{
    public interface IAdminProductService
    {
        public Task<Result<object>> GetAllAsync(AdminProductFilterRequestDto request);
        public Task<Result<ProductDetailResponseDto>> GetByIdAsync(Guid id);
        public Task<Result<ProductDetailResponseDto>> AddAsync(CreateProductRequestDto productRequestDto);
        public Task<Result<bool>> UpdateAsync(Guid id, UpdateProductRequestDto productRequestDto);
        public Task<Result<bool>> SoftDeleteAsync(Guid id);
    }
}
