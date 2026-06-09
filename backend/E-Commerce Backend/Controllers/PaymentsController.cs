using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Payments.Requests;
using E_Commerce_Backend.DTOs.Payments.Responses;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Services.Payments.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("initiate")]
        public async Task<ActionResult<ApiResponse<PaymentInitiateResponseDto>>> Initiate(
            [FromBody] InitiatePaymentRequestDto request,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ApiResponse<PaymentInitiateResponseDto>
                {
                    Success = false,
                    Message = "Unauthorized user."
                });
            }

            var isAdmin = User.IsInRole(UserRole.Admin.ToString());

            var (result, statusCode) = await _paymentService.InitiatePaymentAsync(
                request,
                userId,
                isAdmin,
                cancellationToken);

            if (result.Success)
            {
                return Ok(new ApiResponse<PaymentInitiateResponseDto>
                {
                    Success = true,
                    Message = result.Message,
                    Data = result.Data
                });
            }

            return StatusCode(statusCode, new ApiResponse<PaymentInitiateResponseDto>
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors ?? new List<string>()
            });
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<ActionResult<ApiResponse>> StripeWebhook(CancellationToken cancellationToken)
        {
            string payload;
            using (var reader = new StreamReader(Request.Body))
            {
                payload = await reader.ReadToEndAsync(cancellationToken);
            }

            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            var (result, statusCode) = await _paymentService.HandleStripeWebhookAsync(
                payload,
                stripeSignature,
                cancellationToken);

            if (result.Success)
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = result.Message
                });
            }

            return StatusCode(statusCode, new ApiResponse
            {
                Success = false,
                Message = result.Message,
                Errors = result.Errors ?? new List<string>()
            });
        }
    }
}
