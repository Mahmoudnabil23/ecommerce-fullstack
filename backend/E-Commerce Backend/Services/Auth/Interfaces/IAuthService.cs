using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Auth.Requests;
using E_Commerce_Backend.DTOs.Auth.Responses;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Backend.Services.AuthServices.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto);
        public Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto);
        public Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
        public Task<Result<object>> LogoutAsync(string userId, LogoutRequestDto logoutRequestDto);
        public Task<Result<object>> ChangePasswordAsync(string userId, ChangePasswordRequestDto changePasswordRequestDto);
        public Task<Result<AuthResponseDto>> GoogleAuthAsync(GoogleAuthRequestDto googleAuthRequestDto);
        public Task<Result<object>> VerifyEmailAsync(string userId, string token);
        public Task<Result<object>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);
        public Task<(Result<object>, int)> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);
    }
}
