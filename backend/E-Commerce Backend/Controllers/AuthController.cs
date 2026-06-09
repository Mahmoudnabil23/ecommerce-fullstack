using E_Commerce_Backend.Comman;
using E_Commerce_Backend.Configurations;
using E_Commerce_Backend.DTOs.Auth.Requests;
using E_Commerce_Backend.DTOs.Auth.Responses;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Services.AuthServices.Interfaces;
using E_Commerce_Backend.Services.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace E_Commerce_Backend.Controllers
{

    /*
     TODO : Refactor the AuthController to use the AuthService for handling all authentication related logic, and keep the controller thin by only handling HTTP requests and responses. The AuthService should have methods for login, register, refresh token, and any other authentication related operations. This will improve the separation of concerns and make the code more maintainable and testable.
    TODO: Implement proper error handling and logging in the AuthController and AuthService to ensure that any issues during authentication are properly logged and can be debugged easily. This includes handling exceptions, logging failed login attempts, and providing meaningful error messages to the client.
    TODO: Implement TokenService to handle all token related operations such as generating access tokens, generating refresh tokens, validating tokens, and rotating refresh tokens. This will further improve the separation of concerns and make the code more modular and easier to maintain.
    TODO: Implement Image Upload Module based on cloudinary for user profile pictures, product images, etc. This module should handle image uploading, resizing, and storing the image URLs in the database. It should also provide functionality for deleting images when they are no longer needed.
    TODO: Implement The Remaining Endpoints for the AuthController such as logout, change password, forgot password, reset password, etc. These endpoints should also utilize the AuthService for handling the authentication logic and should follow the same pattern of returning appropriate HTTP status codes and response bodies based on the outcome of the operations.
    TODO: (Done) Refactor The Program.cs to use Seeder classes for seeding the database with initial data such as roles, admin user, etc. This will help in keeping the Program.cs clean and organized, and will also make it easier to manage the seeding process as the application grows.
    TODO: Add Global Exception Handling Middleware and global response formatting to ensure that all responses from the API are consistent and that any unhandled exceptions are properly caught and logged. This will improve the overall robustness and maintainability of the API.
   TODO: Refactor Email Service to work with background jobs
    TODO: Refactor registeration action
    TODO: Refactor reset and forget password
    TODO: Implement change password endpoint
     */


    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IOptions<JwtSettings> jwtSettings, IAuthService authService, IRefreshTokenService refreshTokenService, ITokenService tokenService, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _authService = authService;
            _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto userFromRequest)
        {
            // return 200 if ok with access token and refresh token, else if email or password is incorrect return 401 with error message(unauthorized)

            var result = await _authService.LoginAsync(userFromRequest);

            if (!result.Success)
            {
                return Unauthorized(new ApiResponse<AuthResponseDto>()
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new ApiResponse<AuthResponseDto>()
            {
                Success = true,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto request)
        {
            // return 201 if ok(created at action), else if validation fails return 400 with error details(bad request), else if email already exists return 409 with error message(confilct)

            // 1. validate the request body (data annotations will handle this automatically, we just need to check ModelState.IsValid)
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var response = new ApiResponse<AuthResponseDto>()
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };
                return BadRequest(response);
            }
            // 2. check if email already exists in the database, if exists return 409 with error message (email already exists)


            var existingUser = await _userManager.Users
                            .Where(u => u.Email == request.Email || u.PhoneNumber == request.Phone)
                            .Select(u => new { u.Email, u.PhoneNumber })
                            .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.Email == request.Email)
                {
                    var response = new ApiResponse<RegisterResponseDto>()
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                    return Conflict(response);
                }
                if (existingUser.PhoneNumber == request.Phone)
                {
                    var response = new ApiResponse<RegisterResponseDto>()
                    {
                        Success = false,
                        Message = "Phone number already exists"
                    };
                    return Conflict(response);
                }
            }

            // 3. if not exists, create a new user in the database and return 201 with the new user's id (created at action)
            ApplicationUser newUser = new ApplicationUser()
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                UserName = request.Email // we can use email as username
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, request.Password);
            if (result.Succeeded)
            {
                // user created successfully

                // assign the "Customer" role to the new user
                await _userManager.AddToRoleAsync(newUser, UserRole.Customer.ToString());

                var emailVerificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var frontendBaseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:4200";
                var encodedToken = Uri.EscapeDataString(emailVerificationToken);
                var verificationLink = $"{frontendBaseUrl.TrimEnd('/')}/verify-email?userId={newUser.Id}&token={encodedToken}";
                var emailBody = $"<p>Dear {newUser.FullName},</p>" +
                                "<p>Thank you for registering an account with us. Please click the link below to verify your email address:</p>" +
                                $"<a href='{verificationLink}'>Verify Email</a>" +
                                "<p>If you did not register for an account, please ignore this email.</p>" +
                                "<p>Best regards,<br/>E-Commerce Team</p>";
                await _emailService.SendEmailAsync(new EmailMessage
                {
                    To = newUser.Email,
                    Subject = "Email Verification - E-Commerce",
                    Body = emailBody
                });

                var response = new ApiResponse<RegisterResponseDto>()
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = new RegisterResponseDto() { UserId = newUser.Id }
                };
                return StatusCode(201, response);
            }
            else
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                var response = new ApiResponse<RegisterResponseDto>()
                {
                    Success = false,
                    Message = "User registration failed",
                    Errors = errors
                };
                return BadRequest(response);
            }


        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            // return 200 if token is valid with new access token and new refresh token(for rotating refresh tokens), else if token is invalid return 401 with error message(unauthorized)

            // validate the refresh token
            var result = await _authService.RefreshTokenAsync(request);

            if (!result.Success)
            {
                return Unauthorized(new ApiResponse<RefreshTokenResponseDto>()
                {
                    Success = false,
                    Message = result.Message
                });
            }

            //if (status != RefreshTokenStatus.Valid)
            //{
            //    return Unauthorized(new ApiResponse<RefreshTokenResponseDto>()
            //    {
            //        Success = false,
            //        Message = "Invalid refresh token"
            //    });
            //}

            //// rotate the refresh token
            //string newRefreshToken = await _authService.RotateRefreshTokenAsync(refreshToken);

            // if not failed then we use result to get the new access token and new refresh token and return them in the response

            return Ok(new ApiResponse<RefreshTokenResponseDto>
            {
                Success = result.Success,
                Message = result.Message,
                Data = result.Data
            });
        }

        [HttpGet("verify-email")]
        public async Task<ActionResult<ApiResponse>> VerifyEmail(string userId, string token)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var response = new ApiResponse<AuthResponseDto>()
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };
                return BadRequest(response);
            }

            Result<object> result = await _authService.VerifyEmailAsync(userId, token);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new ApiResponse()
            {
                Success = true,
                Message = result.Message
            });
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var response = new ApiResponse<AuthResponseDto>()
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };
                return BadRequest(response);
            }

            var result = await _authService.ForgotPasswordAsync(request);

            if (!result.Success)
            {
                return BadRequest(new ApiResponse()
                {
                    Success = false,
                    Message = result.Message
                });
            }

            return Ok(new ApiResponse()
            {
                Success = true,
                Message = result.Message
            });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse>> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var response = new ApiResponse<AuthResponseDto>()
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                };
                return BadRequest(response);
            }

            var (result, statusCode) = await _authService.ResetPasswordAsync(request);


            if (!result.Success)
            {
                if (statusCode == 400)
                {
                    return BadRequest(new ApiResponse()
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
                }

                if (statusCode == 422)
                {
                    return UnprocessableEntity(new ApiResponse()
                    {
                        Success = false,
                        Message = result.Message,
                        Errors = result.Errors
                    });
                }

            }

            return Ok(new ApiResponse()
            {
                Success = true,
                Message = result.Message
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse>> Logout([FromBody] LogoutRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse { Success = false, Message = "Unauthorized" });
            }

            var result = await _authService.LogoutAsync(userId, request);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse { Success = false, Message = result.Message, Errors = result.Errors });
            }

            return Ok(new ApiResponse { Success = true, Message = result.Message });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse { Success = false, Message = "Unauthorized" });
            }

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse { Success = false, Message = result.Message, Errors = result.Errors });
            }

            return Ok(new ApiResponse { Success = true, Message = result.Message });
        }

        [HttpPost("google")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Google([FromBody] GoogleAuthRequestDto request)
        {
            var result = await _authService.GoogleAuthAsync(request);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse<AuthResponseDto> { Success = false, Message = result.Message, Errors = result.Errors });
            }

            return Ok(new ApiResponse<AuthResponseDto> { Success = true, Message = result.Message, Data = result.Data });
        }
    }
}
