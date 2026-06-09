using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Admin.Requests;
using E_Commerce_Backend.DTOs.Users.Responses;

namespace E_Commerce_Backend.Services.AdminServices.Interfaces
{
    public interface IAdminUserService
    {
        Task<Result<object>> GetAllAsync(AdminUserFilterRequestDto request);
        Task<Result<AdminUserListItemDto>> GetByIdAsync(Guid id);
        Task<Result<bool>> UpdateStatusAsync(Guid id, UpdateUserStatusRequestDto request);
        Task<Result<bool>> SoftDeleteAsync(Guid id);
    }
}
