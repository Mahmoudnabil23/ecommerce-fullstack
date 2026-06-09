using E_Commerce_Backend.Comman;
using E_Commerce_Backend.Configurations;
using E_Commerce_Backend.DTOs.Auth.Requests;
using E_Commerce_Backend.DTOs.Auth.Responses;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Services.AuthServices.Interfaces;
using E_Commerce_Backend.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System.Text;


namespace E_Commerce_Backend.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings, IRefreshTokenService refreshTokenService, ITokenService tokenService, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _configuration = configuration;
            _emailService = emailService;
        }


        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(loginRequestDto.Email);

            if (user == null)
            {
                return Result<AuthResponseDto>.Fail("Email is not registered");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (!result)
            {
                return Result<AuthResponseDto>.Fail("Incorrect password");
            }

            // if we reach here, it means email and password are correct, we can generate access token and refresh token and return them in the response

            string accessToken = await _tokenService.GenerateAccessToken(user);
            string refreshToken = _tokenService.GenerateRefreshToken();

            var roles = await _userManager.GetRolesAsync(user);


            await _refreshTokenService.StoreAsync(user.Id, refreshToken);

            var response = new AuthResponseDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
                User = new UserSummaryDto()
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Roles = roles.ToList()
                }
            };

            return Result<AuthResponseDto>.Ok(response);
        }



        public Task<Result<RegisterResponseDto>> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            throw new NotImplementedException();
        }


        public async Task<Result<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            (RefreshToken? refreshToken, RefreshTokenStatus status) = await _refreshTokenService.
                                                                      ValidateRefreshTokenAsync(refreshTokenRequestDto.RefreshToken);

            if (status != RefreshTokenStatus.Valid || refreshToken == null)
            {
                return Result<RefreshTokenResponseDto>.Fail("Invalid refresh token");
            }

            // Generate new access token and rotate refresh token
            string newAccessToken = await _tokenService.GenerateAccessToken(refreshToken.User);

            string newRefreshToken = await _refreshTokenService.RotateRefreshTokenAsync(refreshToken);


            var response = new RefreshTokenResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60
            };
            return Result<RefreshTokenResponseDto>.Ok(response);

        }

        public async Task<Result<object>> LogoutAsync(string userId, LogoutRequestDto logoutRequestDto)
        {
            if (string.IsNullOrWhiteSpace(logoutRequestDto.RefreshToken))
            {
                return Result<object>.Fail("Refresh token is required");
            }

            var revoked = await _refreshTokenService.RevokeRefreshTokenAsync(logoutRequestDto.RefreshToken, userId);
            if (!revoked)
            {
                return Result<object>.Fail("Invalid refresh token");
            }

            return Result<object>.Ok(null, "Logged out successfully");
        }

        public async Task<Result<object>> ChangePasswordAsync(string userId, ChangePasswordRequestDto changePasswordRequestDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<object>.Fail("User not found");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordRequestDto.CurrentPassword, changePasswordRequestDto.NewPassword);
            if (!result.Succeeded)
            {
                return Result<object>.Fail("Failed to change password", result.Errors.Select(e => e.Description).ToList());
            }

            return Result<object>.Ok(null, "Password changed successfully");
        }

        public Task<Result<AuthResponseDto>> GoogleAuthAsync(GoogleAuthRequestDto googleAuthRequestDto)
        {
            return Task.FromResult(Result<AuthResponseDto>.Fail("Google authentication is not configured"));
        }

        public async Task<Result<object>> VerifyEmailAsync(string userId, string token)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result<object>.Fail("User not found");
            }
            if (user.EmailConfirmed)
            {
                return Result<object>.Fail("Email is already confirmed");
            }

            // Verify the email using the token
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return Result<object>.Fail("Invalid or expired token");
            }

            return Result<object>.Ok(null, "Email verified successfully");
        }

        public async Task<Result<object>> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);
            if (user == null || !user.EmailConfirmed)
            {
                // To prevent email enumeration attacks, we return the same response whether the user exists or not
                return Result<object>.Ok(null, "If an account with that email exists, a password reset link has been sent.");
            }

            // Generate password reset token
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // send the reset token to the user's email

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));
            var frontendUrl = _configuration["Frontend:ResetPasswordUrl"];
            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                var baseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";
                frontendUrl = $"{baseUrl.TrimEnd('/')}/reset-password";
            }

            var resetlink = $"{frontendUrl}?email={forgotPasswordRequestDto.Email}&token={encodedToken}";

            var emailMessage = new EmailMessage()
            {
                To = forgotPasswordRequestDto.Email,
                Subject = "Password Reset Request",
                Body = $"Click the following link to reset your password: {resetlink}"
            };

            await _emailService.SendEmailAsync(emailMessage);

            return Result<object>.Ok(null, "If an account with that email exists, a password reset link has been sent.");

        }
        public async Task<(Result<object>, int)> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            // First, we need to find the user by email
            // reset password using user and token and new password
            // if password is not met the requirements, return error message with the list of errors with 422
            // if token is invalid or expired, return error message with 400

            ApplicationUser user = await _userManager.FindByEmailAsync(resetPasswordRequestDto.Email);

            if (user == null || !user.EmailConfirmed)
            {
                return (Result<object>.Fail("Invalid email"), 400);
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordRequestDto.Token));


            IdentityResult result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordRequestDto.NewPassword);

            if (!result.Succeeded)
            {

                if (result.Errors.Any(e => e.Code == "InvalidToken"))
                {
                    return (Result<object>.Fail("Invalid or expired token"), 400);
                }
                // If we reach here, it means there are validation errors with the new password
                var errors = result.Errors.Select(e => e.Description).ToList();
                return (Result<object>.Fail("Password does not meet requirements", errors), 422);

            }


            // password reseted successfully
            return (Result<object>.Ok(null, "Password reset successfully"), 200);

        }
    }



}

