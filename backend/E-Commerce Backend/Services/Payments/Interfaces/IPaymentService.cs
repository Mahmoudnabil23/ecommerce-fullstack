using E_Commerce_Backend.Comman;
using E_Commerce_Backend.DTOs.Payments.Requests;
using E_Commerce_Backend.DTOs.Payments.Responses;

namespace E_Commerce_Backend.Services.Payments.Interfaces
{
    public interface IPaymentService
    {
        Task<(Result<PaymentInitiateResponseDto> Result, int StatusCode)> InitiatePaymentAsync(
            InitiatePaymentRequestDto request,
            string userId,
            bool isAdmin,
            CancellationToken cancellationToken = default);

        Task<(Result<object> Result, int StatusCode)> HandleStripeWebhookAsync(
            string payload,
            string stripeSignature,
            CancellationToken cancellationToken = default);
    }
}
