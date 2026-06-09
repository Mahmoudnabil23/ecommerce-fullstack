using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;

namespace E_Commerce_Backend.Services.AuthServices.Interfaces
{
    public interface IRefreshTokenService
    {
        public Task<(RefreshToken?, RefreshTokenStatus)> ValidateRefreshTokenAsync(string token);
        public Task<bool> RevokeRefreshTokenAsync(string token, string userId);
        public Task StoreAsync(string userId, string token);
        public Task<string> RotateRefreshTokenAsync(RefreshToken oldToken);
    }
}
