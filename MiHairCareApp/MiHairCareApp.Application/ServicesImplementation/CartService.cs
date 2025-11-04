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

                var cart = await _unitOfWork.CartRepository.GetCartWithItemsByUserIdAsync(userId) ?? new Cart { UserId = userId };


                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                        Items = new List<CartItem>() // ✅ Initialize items
                    };
                    await _unitOfWork.CartRepository.AddAsync(cart);
                }

                var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
                if (product == null)
                    return ApiResponse<CartViewDto>.Failed("Product not found", 404, new List<string> { });

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
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
                        ProductId = i.ProductId,
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




        public async Task<ApiResponse<StripeResultDto>> CheckoutAsync(string userId)
        {
            try
            {
                var cart = await _unitOfWork.CartRepository.GetByIdAsync(userId);  // GetCartWithItemsByUserIdAsync
                if (cart == null || !cart.Items.Any())
                    return ApiResponse<StripeResultDto>.Failed("Cart is empty", 400, new List<string>());

                var totalAmount = (long)(cart.Items.Sum(i => i.TotalPrice) * 100); // Stripe expects cents

                var options = new PaymentIntentCreateOptions
                {
                    Amount = totalAmount,
                    Currency = "Dollar",
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                    },
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // Optionally persist order record
                var order = new UserTransaction
                {
                    PaymentSucceeded = false, // will be updated on webhook
                    Amount = totalAmount,
                    Currency = Currency.Dollar,
                    Description = "Haircare Products Checkout",
                    CustomerId = userId,
                    CreatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.TransactionRepository.AddAsync(order);
                await _unitOfWork.SaveChangesAsync();

                return ApiResponse<StripeResultDto>.Success(
                    new StripeResultDto { ClientSecret = paymentIntent.ClientSecret },
                    "Checkout initiated",
                    200
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed");
                return ApiResponse<StripeResultDto>.Failed("Checkout failed", 500, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<StripeResultDto>> StripePay(StripePayDto stripePayDto)
        {
            try
            {
                var customers = new CustomerService();
                var charges = new ChargeService();

                // Create the customer
                var customerCreateOptions = new CustomerCreateOptions
                {
                    Email = stripePayDto.StripeEmail,
                    Source = stripePayDto.StripeToken,
                };
                var customer = await customers.CreateAsync(customerCreateOptions);

                // Create the charge
                var chargeCreateOptions = new ChargeCreateOptions
                {
                    Amount = stripePayDto.Amount,
                    Description = stripePayDto.Description,
                    Currency = stripePayDto.Currency,
                    Customer = customer.Id,
                };
                var charge = await charges.CreateAsync(chargeCreateOptions);

                if (charge != null)
                {
                    var currencyEnum = Enum.TryParse<Currency>(
                                            charge.Currency,   // string from Stripe, e.g. "usd"
                                            true,              // ignore case
                                            out var parsedCurrency
                                        ) ? parsedCurrency : Currency.Unknown; // fallback if parsing fails

                    var userTransaction = new UserTransaction
                    {
                        PaymentSucceeded = charge.Status == "succeeded",
                        Amount = charge.Amount,
                        CreatedAt = DateTime.UtcNow,
                        Currency = currencyEnum,
                        Description = charge.Description,
                        CustomerId = customer.Id,
                        PaymentReference = charge.BalanceTransactionId,
                        ReceiptEmail = charge.ReceiptEmail,
                    };


                    await _unitOfWork.TransactionRepository.AddAsync(userTransaction);
                    await _unitOfWork.SaveChangesAsync();

                    var stripeResultDto = new StripeResultDto
                    {
                        PaymentSucceeded = userTransaction.PaymentSucceeded,
                        Amount = userTransaction.Amount,
                        Currency = userTransaction.Currency.ToString(),
                        Description = userTransaction.Description,
                        CustomerId = userTransaction.CustomerId,
                        PaymentReference = userTransaction.PaymentReference
                    };

                    stripeResultDto = _mapper.Map<StripeResultDto>(userTransaction);

                    return ApiResponse<StripeResultDto>.Success(stripeResultDto, "Payment succeeded", 200);
                }
                else
                {
                    return ApiResponse<StripeResultDto>.Failed("Payment failed", 400, new List<string> { "Charge creation failed" });
                }
            }
            catch (StripeException ex)
            {
                return ApiResponse<StripeResultDto>.Failed("Stripe error", 500, new List<string> { ex.Message });
            }
            catch (Exception ex)
            {
                return ApiResponse<StripeResultDto>.Failed("Server error", 500, new List<string> { ex.Message });
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
