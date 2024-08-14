using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Application.ServicesImplementation;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    public class UsersController : ControllerBase
    {
          private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            
            _userService = userService;
        }


        [HttpGet("id")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            return Ok(await _userService.GetUserById(userId));
        }



        [HttpGet("regular-users")]
        public async Task<IActionResult> GetStylists()
        {
            var users = await _userService.GetUsersWithNullCompanyNameAsync();
            return Ok(users);
        }



        [HttpGet("stylist-users")]
        public async Task<IActionResult> GetUsersWithCompany()
        {
            var response = await _userService.GetStylistUsers();
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }





        [HttpGet("regUsers")]
        public async Task<IActionResult> GetRegularUsers()
        {
            return Ok(await _userService.GetUsersWithNullCompanyNameAsync());
        }



        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto([FromForm] UpdatePhotoDto updatePhotoDto)
        {
            return Ok(await _userService.AddPhoto(updatePhotoDto));
        }



        [HttpGet("getPhoto")]
        public async Task<IActionResult> ViewPhoto(string photoId)
        {
            var photoResponse = await _userService.GetPhoto(photoId);

            if (photoResponse.Succeeded)
            {
                return Ok(photoResponse);
            }
            else
            {
                return BadRequest(photoResponse);
            }
        }


         

        [HttpDelete("delete-photo")]
        public async Task<IActionResult> DeletePhoto(string photoId)
        {
            return Ok(await _userService.DeletePhotoAsync(photoId));
        }




        [HttpDelete("stripePay")]
        public async Task<IActionResult> PayByStripe(StripePayDto stripePayDto)
        {
            return Ok(await _userService.StripePay(stripePayDto));
        }




    }
}
