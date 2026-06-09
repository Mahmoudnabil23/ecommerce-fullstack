using E_Commerce_Backend.Configurations;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Repositories.Interfaces;
using E_Commerce_Backend.Services.AuthServices.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace E_Commerce_Backend.Services.AuthServices
{
    public class RefreshTokenService : IRefreshTokenService
    {

        private readonly IRefreshTokenRepository _refreshToken;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;

        public RefreshTokenService(IRefreshTokenRepository refreshToken, IOptions<JwtSettings> jwtSettings, ITokenService tokenService)
        {
            _refreshToken = refreshToken;
            _jwtSettings = jwtSettings.Value;
            _tokenService = tokenService;
        }


        // TODO: replace tuple by result pattern
        public async Task<(RefreshToken?, RefreshTokenStatus)> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshToken.GetByTokenAsync(token);

            if (refreshToken == null)
            {
                return (null, RefreshTokenStatus.NotFound);
            }
            if (refreshToken.IsRevoked)
            {
                return (refreshToken, RefreshTokenStatus.Revoked);
            }
            if (refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return (refreshToken, RefreshTokenStatus.Expired);
            }

            return (refreshToken, RefreshTokenStatus.Valid);
        }

        public async Task StoreAsync(string userId, string token)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            await _refreshToken.AddAsync(refreshToken);
            await _refreshToken.SaveChangesAsync();
        }

        public async Task<bool> RevokeRefreshTokenAsync(string token, string userId)
        {
            var refreshToken = await _refreshToken.GetByTokenAsync(token);
            if (refreshToken == null || refreshToken.UserId != userId || refreshToken.IsRevoked)
            {
                return false;
            }

            refreshToken.IsRevoked = true;
            await _refreshToken.SaveChangesAsync();
            return true;
        }

        public async Task<string> RotateRefreshTokenAsync(RefreshToken oldToken)
        {
            // revoke the old token
            oldToken.IsRevoked = true;
            await _refreshToken.SaveChangesAsync();

            // generate a new token
            string newToken = _tokenService.GenerateRefreshToken();

            // create a new refresh token record in the database
            var newRefreshToken = new RefreshToken
            {
                UserId = oldToken.UserId,
                Token = newToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _refreshToken.AddAsync(newRefreshToken);
            await _refreshToken.SaveChangesAsync();

            return newToken;
        }


    }
}
