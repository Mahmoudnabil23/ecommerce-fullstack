using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Repositories.Interfaces
{
    // Methods required by: §1.1 auth flows, §1.2 profile, §1.10 admin user mgmt

  
        public interface IUserRepository : IGenericRepository<ApplicationUser>
        {
            // Required by: POST /auth/login, POST /auth/register duplicate check
            Task<ApplicationUser?> GetByEmailAsync(string email);

            // Required by: POST /auth/register duplicate check
            Task<ApplicationUser?> GetByPhoneAsync(string phone);

            // Required by: GET /auth/verify-email?token={token}
            Task<ApplicationUser?> GetByEmailVerificationTokenAsync(string token);

            // Required by: POST /auth/reset-password
            Task<ApplicationUser?> GetByPasswordResetTokenAsync(string token);

            // Required by: POST /auth/refresh-token, POST /auth/logout
            Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken);

            // Required by: GET /admin/users with role/status/search filters + pagination
            Task<(List<ApplicationUser> Users, int Total)> GetPagedAsync(
                UserRole? role,
                UserStatus? status,
                string? search,
                int page,
                int limit);
        }
    }
