using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<ApiResponse<CartDto>> GetCartAsync(string userId);
        Task<ApiResponse<StripeResultDto>> StripePay(StripePayDto stripePayDto);
        Task<ApiResponse<CartViewDto>> AddToCartAsync(string userId, AddToCartDto dto);
        Task<ApiResponse<StripeResultDto>> CheckoutAsync(string userId);
        Task<ApiResponse<bool>> RemoveItemAsync(string userId, string productId);
        Task<ApiResponse<bool>> ClearCartAsync(string userId);

    }
}
