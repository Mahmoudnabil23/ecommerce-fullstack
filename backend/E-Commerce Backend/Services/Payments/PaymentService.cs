using E_Commerce_Backend.Comman;
using E_Commerce_Backend.Configurations;
using E_Commerce_Backend.DTOs.Payments.Requests;
using E_Commerce_Backend.DTOs.Payments.Responses;
using E_Commerce_Backend.Enums;
using E_Commerce_Backend.Models;
using E_Commerce_Backend.Services.Payments.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentMethodEnum = E_Commerce_Backend.Enums.PaymentMethod;
using Stripe;

namespace E_Commerce_Backend.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly StripeSettings _stripeSettings;

        public PaymentService(AppDbContext context, IOptions<StripeSettings> stripeOptions)
        {
            _context = context;
            _stripeSettings = stripeOptions.Value;

            if (!string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
            {
                StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
            }
        }

        public async Task<(Result<PaymentInitiateResponseDto> Result, int StatusCode)> InitiatePaymentAsync(
            InitiatePaymentRequestDto request,
            string userId,
            bool isAdmin,
            CancellationToken cancellationToken = default)
        {
            if (request.OrderId == Guid.Empty)
            {
                return (Result<PaymentInitiateResponseDto>.Fail("OrderId is required."), 400);
            }

            if (request.PaymentMethod != PaymentMethodEnum.Stripe)
            {
                return (Result<PaymentInitiateResponseDto>.Fail("Only Stripe payment method is supported by this endpoint."), 400);
            }

            if (string.IsNullOrWhiteSpace(_stripeSettings.SecretKey))
            {
                return (Result<PaymentInitiateResponseDto>.Fail("Stripe is not configured on the server."), 500);
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
            {
                return (Result<PaymentInitiateResponseDto>.Fail("Order not found."), 404);
            }

            if (!isAdmin && !string.Equals(order.UserId, userId, StringComparison.Ordinal))
            {
                return (Result<PaymentInitiateResponseDto>.Fail("You are not allowed to initiate payment for this order."), 403);
            }

            if (order.PaymentMethod != PaymentMethodEnum.Stripe)
            {
                return (Result<PaymentInitiateResponseDto>.Fail("This order is not configured for Stripe payment."), 409);
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                return (Result<PaymentInitiateResponseDto>.Fail("Order is already paid."), 409);
            }

            if (order.Total <= 0)
            {
                return (Result<PaymentInitiateResponseDto>.Fail("Order total must be greater than zero."), 400);
            }

            var currency = string.IsNullOrWhiteSpace(request.Currency)
                ? _stripeSettings.DefaultCurrency
                : request.Currency;

            var amountInMinorUnits = Convert.ToInt64(
                decimal.Round(order.Total * 100m, 0, MidpointRounding.AwayFromZero));

            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = amountInMinorUnits,
                Currency = currency.ToLowerInvariant(),
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
                {
                    ["orderId"] = order.Id.ToString(),
                    ["orderNumber"] = order.OrderNumber,
                    ["userId"] = order.UserId ?? string.Empty
                }
            };

            var paymentIntentService = new PaymentIntentService();
            var paymentIntent = await paymentIntentService.CreateAsync(
                paymentIntentOptions,
                new RequestOptions
                {
                    IdempotencyKey = $"payment-intent-order-{order.Id}"
                },
                cancellationToken);

            var response = new PaymentInitiateResponseDto
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            };

            return (Result<PaymentInitiateResponseDto>.Ok(response, "Payment intent created successfully."), 200);
        }

        public async Task<(Result<object> Result, int StatusCode)> HandleStripeWebhookAsync(
            string payload,
            string stripeSignature,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_stripeSettings.WebhookSecret))
            {
                return (Result<object>.Fail("Stripe webhook is not configured on the server."), 500);
            }

            if (string.IsNullOrWhiteSpace(stripeSignature))
            {
                return (Result<object>.Fail("Missing Stripe signature header."), 400);
            }

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(payload, stripeSignature, _stripeSettings.WebhookSecret);
            }
            catch (StripeException)
            {
                return (Result<object>.Fail("Invalid Stripe webhook signature."), 400);
            }
            catch
            {
                return (Result<object>.Fail("Invalid Stripe webhook payload."), 400);
            }

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await UpdateOrderPaymentStatusAsync(paymentIntent, PaymentStatus.Paid, cancellationToken);
                    break;
                }
                case "payment_intent.payment_failed":
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    await UpdateOrderPaymentStatusAsync(paymentIntent, PaymentStatus.Failed, cancellationToken);
                    break;
                }
            }

            return (Result<object>.Ok(null!, "Webhook processed successfully."), 200);
        }

        private async Task UpdateOrderPaymentStatusAsync(
            PaymentIntent? paymentIntent,
            PaymentStatus paymentStatus,
            CancellationToken cancellationToken)
        {
            if (paymentIntent?.Metadata == null ||
                !paymentIntent.Metadata.TryGetValue("orderId", out var orderIdRaw) ||
                !Guid.TryParse(orderIdRaw, out var orderId))
            {
                return;
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order == null)
            {
                return;
            }

            if (order.PaymentStatus == paymentStatus)
            {
                return;
            }

            order.PaymentStatus = paymentStatus;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
