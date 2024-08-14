using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;

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


        [HttpGet("getAllBooking")]
        public async Task<ActionResult> SeeAllBookings()
        {
            var allBookings = await _bookingService.GetAllBookingsAsync();
            return Ok(allBookings);
        }

        [HttpPut("updateBooking")]
        public async Task<ActionResult> UpsertBooking(UpdateBookingDto updateBooking)
        {
            var update = await _bookingService.UpdateBookingAsync(updateBooking);
            return Ok(update);
        }


        [HttpDelete("deleteBooking")]
        public async Task<ActionResult> UndoBooking(string bookinId)
        {
            var deleted = await _bookingService.DeleteABookingAsync(bookinId);
            return Ok(deleted);
        }


    }
}
