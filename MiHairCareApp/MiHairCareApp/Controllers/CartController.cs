using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using System.Security.Claims;

namespace MiHairCareApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        //[Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromQuery] string userId, [FromBody] AddToCartDto dto)
        {
            var result = await _cartService.AddToCartAsync(userId, dto);
            return Ok(result);
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(string userId)
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _cartService.GetCartAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest req)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);                                                                          

            var result = await _cartService.CheckoutAsync(userId, req.PaymentIntentId);
            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete("{userId}/item/{productId}")]
        public async Task<IActionResult> RemoveItem(string userId, string productId)
        {
            var result = await _cartService.RemoveItemAsync(userId, productId);
            return StatusCode(result.StatusCode, result);
        }

        // DELETE api/cart/{userId}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> ClearCart(string userId)
        {
            var result = await _cartService.ClearCartAsync(userId);
            return StatusCode(result.StatusCode, result);
        }

       


    }

}
