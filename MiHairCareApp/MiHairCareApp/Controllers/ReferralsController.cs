using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Application.ServicesImplementation;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferralsController : ControllerBase
    {
        private readonly IReferralService _referralService;

        public ReferralsController(IReferralService referralService)
        {
            _referralService = referralService;
        }



        [HttpPost("createReferral")]
        public async Task<ActionResult> MakeReferral(CreateReferralDto referralDto)
        {
            var referResult = await _referralService.CreateReferralAsync(referralDto);
            if (referResult == null)
            {
                return BadRequest();
            }
            return Ok(referResult);
        }

        [HttpGet("getById")]
        public async Task<ActionResult> GetReferralById(string bookingId)
        {
            var referral = await _referralService.GetReferralByIdAsync(bookingId);
            if (referral == null)
            {
                return BadRequest();
            }
            return Ok(referral);
        }


        [HttpGet("getAllReferals")]
        public async Task<ActionResult> SeeAllReferrals()
        {
            var allReferals = await _referralService.GetAllReferralsAsync();
            return Ok(allReferals);
        }

         


        [HttpDelete("deleteReferral")]
        public async Task<ActionResult> UndoBooking(string referralId)
        {
            var deleted = await _referralService.DeleteABookingAsync(referralId);
            return Ok(deleted);
        }






    }
}
