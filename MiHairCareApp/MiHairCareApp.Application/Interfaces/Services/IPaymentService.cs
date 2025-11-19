using Microsoft.AspNetCore.Http;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IPaymentService
    {
         
        Task HandleStripeWebhookAsync(HttpRequest request);
        Task<ApiResponse<PaymentIntentResponseDto>> CreatePaymentIntentAsync(CreatePaymentRequestDto req);
       
    }



}
