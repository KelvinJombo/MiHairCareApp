using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using Stripe;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [Authorize (Roles = "User")]
        [HttpPost("createBooking")]
        public async Task<ActionResult> BookStylist(CreateBookingDto bookingDto)
        {
            var bookingResult = await _bookingService.CreateBookingAsync(bookingDto);
            if(bookingResult == null)
            {
                return BadRequest();                
            }
            return Ok(bookingResult);
        }


        [Authorize (Roles = "User")]
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] BookingRequestDto request)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return Ok(new { clientSecret = paymentIntent.ClientSecret });
        }





        [Authorize (Roles = "Admin")]
        [HttpGet("getById")]
        public async Task<ActionResult> GetBookingById(string bookingId)
        {
            var booking = await _bookingService.GetBookingByIdAsync(bookingId);
            if(booking == null)
            {
                return BadRequest();    
            }
            return Ok(booking);
        }

        [Authorize (Roles = "Admin")]
        [HttpGet("getAllBooking")]
        public async Task<ActionResult> SeeAllBookings()
        {
            var allBookings = await _bookingService.GetAllBookingsAsync();
            return Ok(allBookings);
        }


        [Authorize (Roles = "User")]
        [HttpPut("updateBooking")]
        public async Task<ActionResult> UpsertBooking(UpdateBookingDto updateBooking)
        {
            var update = await _bookingService.UpdateBookingAsync(updateBooking);
            return Ok(update);
        }

        [Authorize (Roles = "User")]
        [HttpDelete("deleteBooking")]
        public async Task<ActionResult> UndoBooking(string bookinId)
        {
            var deleted = await _bookingService.DeleteABookingAsync(bookinId);
            return Ok(deleted);
        }


    }
}
