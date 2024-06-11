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

         

        [HttpPost("add-photo")]
        public async Task<IActionResult> AddPhoto([FromForm] UpdatePhotoDto updatePhotoDto)
        {
            return Ok(await _userService.AddUserPhoto(updatePhotoDto));
        }



        [HttpDelete("delete-photo")]
        public async Task<IActionResult> DeletePhoto(string UserId )
        {
            return Ok(await _userService.DeleteUserPhotoAsync(UserId));
        }



    }
}
