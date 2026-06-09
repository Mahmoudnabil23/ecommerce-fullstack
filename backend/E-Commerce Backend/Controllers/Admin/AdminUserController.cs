using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Admin.Requests;
using E_Commerce_Backend.DTOs.Users.Responses;
using E_Commerce_Backend.Services.AdminServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Backend.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _userService;

        public AdminUserController(IAdminUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAll([FromQuery] AdminUserFilterRequestDto request)
        {
            Result<object> result = await _userService.GetAllAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Users retrieved successfully.",
                Data = result.Data
            });
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ApiResponse<AdminUserListItemDto>>> GetById(Guid id)
        {
            Result<AdminUserListItemDto> result = await _userService.GetByIdAsync(id);

            if (!result.Success)
            {
                return NotFound(new ApiResponse<AdminUserListItemDto>
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new ApiResponse<AdminUserListItemDto>
            {
                Success = true,
                Message = "User retrieved successfully.",
                Data = result.Data
            });
        }

        [HttpPut("{id:Guid}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateStatus(Guid id, [FromBody] UpdateUserStatusRequestDto request)
        {
            Result<bool> result = await _userService.UpdateStatusAsync(id, request);

            if (!result.Success)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = result.Message
            });
        }

        [HttpDelete("{id:Guid}")]
        public async Task<ActionResult<ApiResponse>> Delete(Guid id)
        {
            Result<bool> result = await _userService.SoftDeleteAsync(id);

            if (!result.Success)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = result.Message,
                    Errors = result.Errors
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = result.Message
            });
        }

    }
}
