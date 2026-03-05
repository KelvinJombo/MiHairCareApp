using AutoMapper;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Exceptions;
using MiHairCareApp.Domain.Enums;
using Stripe;
using Microsoft.AspNetCore.Http;

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
            if (string.IsNullOrWhiteSpace(userId))
                throw new ValidationException("User ID is required.");

            var cart = await _unitOfWork.CartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any())
                return ApiResponse<CartDto>.Failed("Cart is empty", StatusCodes.Status404NotFound, new List<string> { "No items found in cart" });

            var cartDto = _mapper.Map<CartDto>(cart);
            return ApiResponse<CartDto>.Success(cartDto, "Cart retrieved successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<CartViewDto>> AddToCartAsync(string userId, AddToCartDto dto)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ValidationException("User ID is required.");

            var cart = await _unitOfWork.CartRepository.GetCartWithItemsByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                await _unitOfWork.CartRepository.AddAsync(cart);
            }

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.Id);
            if (product == null)
                throw new NotFoundException("Product not found");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.Id);
            if (existingItem != null)
                existingItem.Quantity += dto.Quantity;
            else
            {
                cart.Items.Add(new CartItem { ProductId = product.Id, Quantity = dto.Quantity, UnitPrice = product.Price });
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

            return ApiResponse<CartViewDto>.Success(cartDto, "Item added to cart successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<StripeResultDto>> CheckoutAsync(string userId, string paymentIntentId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ValidationException("User required");

            var cart = await _unitOfWork.CartRepository.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.Items.Any()) throw new ValidationException("Cart is empty");

            var existingTx = await _unitOfWork.TransactionRepository.FindByPaymentIntentIdAsync(paymentIntentId);
            if (existingTx != null && existingTx.PaymentSucceeded) throw new ValidationException("Payment already processed");

            var service = new PaymentIntentService();
            var pi = await service.GetAsync(paymentIntentId);
            if (pi == null) throw new NotFoundException("PaymentIntent not found");

            if (pi.Status != "succeeded") throw new ValidationException($"Payment not completed (status: {pi.Status})");

            var order = new Order
            {
                UserId = userId,
                SubTotal = cart.Items.Sum(i => i.TotalPrice),
                Tax = 0,
                ShippingFee = 0,
                Currency = Currency.Pounds,
                PaymentIntentId = paymentIntentId,
                OrderDate = DateTime.UtcNow,
                Items = cart.Items.Select(i => new OrderItem { ProductId = i.ProductId, Quantity = i.Quantity, UnitPrice = i.UnitPrice }).ToList()
            };

            await _unitOfWork.OrderRepository.AddAsync(order);

            var tx = await _unitOfWork.TransactionRepository.FindByPaymentIntentIdAsync(paymentIntentId);
            if (tx != null)
            {
                tx.PaymentSucceeded = true;
                tx.Status = "succeeded";
                tx.CreatedAt = DateTime.UtcNow;
            }

            cart.Items.Clear();
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<StripeResultDto>.Success(null, "Checkout completed", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<bool>> RemoveItemAsync(string userId, string productId)
        {
            var cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(userId);

            if (cart == null)
                throw new NotFoundException("Cart not found");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new NotFoundException("Item not found in cart");

            cart.Items.Remove(item);
            _unitOfWork.CartRepository.Update(cart);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Item removed successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<bool>> ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.CartRepository.GetCartWithItemsAsync(userId);
            if (cart == null)
                throw new NotFoundException("Cart not found");

            cart.Items.Clear();
            _unitOfWork.CartRepository.Update(cart);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true, "Cart cleared successfully", StatusCodes.Status200OK);
        }
    }
}
