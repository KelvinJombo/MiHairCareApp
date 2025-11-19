using AutoMapper;
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
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly ILogger<CartService> _logger;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, ILogger<CartService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }


        public async Task<ApiResponse<CartDto>> GetCartAsync(string userId)
        {
            try
            {
                var cart = await _unitOfWork.CartRepository.GetCartWithItemsByUserIdAsync(userId);

                if (cart == null || !cart.Items.Any())
                {
                    return ApiResponse<CartDto>.Failed(
                        "Cart is empty",
                        404,
                        new List<string> { "No items found in cart" });
                }

                var cartDto = _mapper.Map<CartDto>(cart);
                return ApiResponse<CartDto>.Success(cartDto, "Cart retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart for user {UserId}", userId);
                return ApiResponse<CartDto>.Failed("Failed to retrieve cart", 500, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<CartViewDto>> AddToCartAsync(string userId, AddToCartDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    return ApiResponse<CartViewDto>.Failed("User ID is required.", 400, new List<string> { });

                var cart = await _unitOfWork.CartRepository.GetCartWithItemsByUserIdAsync(userId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        Items = new List<CartItem>()
                    };

                    await _unitOfWork.CartRepository.AddAsync(cart);
                }


                var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.Id);
                if (product == null)
                    return ApiResponse<CartViewDto>.Failed("Product not found", 404, new List<string> { });

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += dto.Quantity;
                }
                else
                {
                    cart.Items.Add(new CartItem
                    {
                        ProductId = product.Id,
                        Quantity = dto.Quantity,
                        UnitPrice = product.Price
                    });
                }

                await _unitOfWork.SaveChangesAsync();

                var cartDto = new CartViewDto
                {
                    Items = cart.Items.Select(i => new CartItemViewDto
                    {
                        Id = i.ProductId,
                        ProductName = i.Product?.ProductName
                            ?? _unitOfWork.ProductRepository.GetByIdAsync(i.ProductId).Result?.ProductName
                            ?? "Unknown Product",
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList(),
                    TotalAmount = cart.Items.Sum(i => i.TotalPrice)
                };

                return ApiResponse<CartViewDto>.Success(cartDto, "Item added to cart successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart");
                return ApiResponse<CartViewDto>.Failed("Error adding item to cart", 500, new List<string> { ex.Message });
            }
        }




        public async Task<ApiResponse<StripeResultDto>> CheckoutAsync(string userId, string paymentIntentId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return ApiResponse<StripeResultDto>.Failed("User required", 400, new List<string>());

                var cart = await _unitOfWork.CartRepository.GetCartWithItemsByUserIdAsync(userId);
                if (cart == null || !cart.Items.Any()) return ApiResponse<StripeResultDto>.Failed("Cart is empty", 400, new List<string>());

                // Prevent double-finalization
                var existingTx = await _unitOfWork.TransactionRepository.FindByPaymentIntentIdAsync(paymentIntentId);
                if (existingTx != null && existingTx.PaymentSucceeded)
                    return ApiResponse<StripeResultDto>.Failed("Payment already processed", 400, new List<string>());

                // Verify payment intent with Stripe
                var service = new PaymentIntentService();
                var pi = await service.GetAsync(paymentIntentId);
                if (pi == null) return ApiResponse<StripeResultDto>.Failed("PaymentIntent not found", 400, new List<string>());

                if (pi.Status != "succeeded")
                {
                    // If not succeeded, return informative result; webhook could handle success asynchronously
                    return ApiResponse<StripeResultDto>.Failed($"Payment not completed (status: {pi.Status})", 400, new List<string>());
                }


                var order = new Order
                {
                    UserId = userId,  // string now
                    SubTotal = cart.Items.Sum(i => i.TotalPrice),
                    Tax = 0,
                    ShippingFee = 0,
                    Currency = Currency.Pounds,
                    PaymentIntentId = paymentIntentId,
                    OrderDate = DateTime.UtcNow,
                    Items = cart.Items.Select(i => new OrderItem
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                };


                await _unitOfWork.OrderRepository.AddAsync(order);

                // mark transaction associated with paymentIntentId as succeeded (if exists)
                var tx = await _unitOfWork.TransactionRepository.FindByPaymentIntentIdAsync(paymentIntentId);
                if (tx != null)
                {
                    tx.PaymentSucceeded = true;
                    tx.Status = "succeeded";
                    tx.CreatedAt = DateTime.UtcNow;
                }

                // Clear cart items
                cart.Items.Clear();

                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<StripeResultDto>.Success(null, "Checkout completed", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed");
                return ApiResponse<StripeResultDto>.Failed("Checkout failed", 500, new List<string> { ex.Message });
            }
        }




        public async Task<ApiResponse<bool>> RemoveItemAsync(string userId, string productId)
        {
            try
            {
                var cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(userId);

                if (cart == null)
                    return ApiResponse<bool>.Failed("Cart not found", 404, new List<string>());

                var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

                if (item == null)
                    return ApiResponse<bool>.Failed("Item not found in cart", 404, new List<string>());

                cart.Items.Remove(item);

                _unitOfWork.CartRepository.Update(cart);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Item removed successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {ProductId} from cart for user {UserId}", productId, userId);
                return ApiResponse<bool>.Failed("Failed to remove item", 500, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> ClearCartAsync(string userId)
        {
            try
            {
                var cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(userId);

                if (cart == null)
                    return ApiResponse<bool>.Failed("Cart not found", 404, new List<string>());

                cart.Items.Clear();

                _unitOfWork.CartRepository.Update(cart);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<bool>.Success(true, "Cart cleared successfully", 200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for user {UserId}", userId);
                return ApiResponse<bool>.Failed("Failed to clear cart", 500, new List<string> { ex.Message });
            }
        }





    }
}
