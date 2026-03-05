using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Enums;
using Stripe;

 


namespace MiHairCareApp.Application.ServicesImplementation
{
    public class PaymentService : IPaymentService
    {
        private readonly PaymentIntentService _paymentIntentService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _webhookSecret;


        public PaymentService(ILogger<PaymentService> logger, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _paymentIntentService = new PaymentIntentService();
            _logger = logger;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _webhookSecret = configuration["Stripe:WebhookSecret"]!;
        }

        public async Task<ApiResponse<PaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentRequestDto req)
        {
            try
            {
                // Set default currency to NGN if not specified
                string currency = req.Currency?.ToLower() ?? "ngn";

                // Validate NGN-specific requirements
                if (currency == "ngn")
                {
                    if (req.Amount < 500 || req.Amount > 100000000)
                    {
                        return ApiResponse<PaymentIntentResponseDto>.Failed(
                            "NGN amount must be between 500 and 100,000,000",
                            400,
                            new List<string> { "Invalid amount range for NGN" }
                        );
                    }
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = req.Amount,   
                    Currency = currency,
                    ReceiptEmail = req.Customer?.Email,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                    Metadata = new Dictionary<string, string>
            {
                { "customer_name", req.Customer?.Name ?? "" },
                { "customer_email", req.Customer?.Email ?? "" },
            }
                };

                var paymentIntent = await _paymentIntentService.CreateAsync(options);

                // Save pending transaction - update to handle NGN
                var transaction = new UserTransaction
                {
                    PaymentIntentId = paymentIntent.Id,
                    PaymentSucceeded = false,
                    Amount = req.Amount,
                    Currency = currency == "ngn" ? Currency.NGN : Currency.Pounds, 
                    Description = "Checkout initiated",
                    CustomerEmail = req.Customer?.Email!,
                    CustomerName = req.Customer?.Name!,
                    CustomerPhone = req.Customer?.Phone!,
                    CreatedAt = DateTime.UtcNow,
                    Status = "pending"
                };

                await _unitOfWork.TransactionRepository.AddAsync(transaction);
                await _unitOfWork.SaveChangesAsync();

                var dto = new PaymentIntentResponseDto
                {
                    ClientSecret = paymentIntent.ClientSecret,
                    PaymentIntentId = paymentIntent.Id
                };

                return ApiResponse<PaymentIntentResponseDto>.Success(
                    dto,
                    "Payment intent created successfully",
                    200
                );
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe API error during PaymentIntent creation.");
                return ApiResponse<PaymentIntentResponseDto>.Failed(
                    $"Stripe error: {ex.Message}",
                    500,
                    new List<string> { ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during PaymentIntent creation.");
                return ApiResponse<PaymentIntentResponseDto>.Failed(
                    $"Server error: {ex.Message}",
                    500,
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task HandleStripeWebhookAsync(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    request.Headers["Stripe-Signature"],
                    _webhookSecret
                );

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        await HandlePaymentSucceeded(paymentIntent);
                        break;

                    case "payment_intent.payment_failed":
                        var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                        await HandlePaymentFailed(failedIntent);
                        break;

                    default:
                        _logger.LogInformation($"Unhandled Stripe event type: {stripeEvent.Type}");
                        break;
                }

            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "⚠️ Stripe webhook verification failed.");
                throw new ApplicationException("Invalid Stripe webhook signature.");
            }
        }

        private async Task HandlePaymentSucceeded(PaymentIntent paymentIntent)
        {
            _logger.LogInformation($"✅ Payment succeeded for intent: {paymentIntent.Id}");

            var record = new SalesRecord
            {
                PaymentIntentId = paymentIntent.Id,
                Amount = paymentIntent.Amount / 100m,
                Currency = paymentIntent.Currency.ToUpper(),
                Email = paymentIntent.ReceiptEmail ?? "unknown",
                Description = paymentIntent.Description ?? "N/A",
                PaymentDate = DateTime.UtcNow,
                Status = "Succeeded"
            };

            await _unitOfWork.SalesRecordRepository.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task HandlePaymentFailed(PaymentIntent paymentIntent)
        {
            _logger.LogWarning($"❌ Payment failed for intent: {paymentIntent.Id}");

            var record = new SalesRecord
            {
                PaymentIntentId = paymentIntent.Id,
                Amount = paymentIntent.Amount / 100m,
                Currency = paymentIntent.Currency.ToUpper(),
                Email = paymentIntent.ReceiptEmail ?? "unknown",
                Description = paymentIntent.Description ?? "N/A",
                PaymentDate = DateTime.UtcNow,
                Status = "Failed"
            };

            await _unitOfWork.SalesRecordRepository.AddAsync(record);
            await _unitOfWork.SaveChangesAsync();
        }
    }





}



