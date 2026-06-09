using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Admin.Requests;
using E_Commerce_Backend.DTOs.Users.Responses;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories;
using E_Commerce_Backend.Services.AdminServices.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Backend.Services.AdminServices
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminUserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<object>> GetAllAsync(AdminUserFilterRequestDto request)
        {
            IQueryable<ApplicationUser> query = _unitOfWork.Users.GetAll();

            if (request.Status.HasValue)
            {
                query = query.Where(u => u.Status == request.Status.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(u => u.FullName.Contains(request.Search) || u.Email.Contains(request.Search));
            }

            int totalCount = await query.CountAsync();

            List<ApplicationUser> pagedUsers = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((request.Page - 1) * request.Limit)
                .Take(request.Limit)
                .ToListAsync();

            List<AdminUserListItemDto> usersDto = new List<AdminUserListItemDto>();

            foreach (ApplicationUser user in pagedUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usersDto.Add(new AdminUserListItemDto()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Status = user.Status,
                    CreatedAt = user.CreatedAt
                });
            }

            var pagedData = new
            {
                TotalItems = totalCount,
                CurrentPage = request.Page,
                PageSize = request.Limit,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.Limit),
                Users = usersDto
            };

            return Result<object>.Ok(pagedData);
        }

        public async Task<Result<AdminUserListItemDto>> GetByIdAsync(Guid id)
        {
            ApplicationUser? user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return Result<AdminUserListItemDto>.Fail($"User with ID {id} was not found.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            AdminUserListItemDto userDto = new AdminUserListItemDto()
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Roles = roles.ToList(),
                Status = user.Status,
                CreatedAt = user.CreatedAt
            };

            return Result<AdminUserListItemDto>.Ok(userDto);
        }

        public async Task<Result<bool>> UpdateStatusAsync(Guid id, UpdateUserStatusRequestDto request)
        {
            ApplicationUser? user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return Result<bool>.Fail($"User with ID {id} was not found.");
            }

            user.Status = request.Status;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "User status updated successfully.");
        }

        public async Task<Result<bool>> SoftDeleteAsync(Guid id)
        {
            ApplicationUser? user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
            {
                return Result<bool>.Fail($"User with ID {id} was not found.");
            }
            user.IsDeleted = true;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "User deleted successfully.");
        }
    }
}
