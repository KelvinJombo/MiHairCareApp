using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HairStylesController : ControllerBase
    {
        private readonly IHairStyleServices _hairStyleServices;

        public HairStylesController(IHairStyleServices hairStyleServices)
        {
            _hairStyleServices = hairStyleServices;
        }



        [HttpPost("addHairStyle")]
        public async Task<IActionResult> AddHairStyle(CreateHairStyleDto createHairStyleDto)
        {
            return Ok(await _hairStyleServices.AddHairStyleAsync(createHairStyleDto));
        }






        [HttpGet("id")]
        public async Task<IActionResult> GetUserById(string hairStyleId)
        {
            return Ok(await _hairStyleServices.GetHairStyleById(hairStyleId));
        }



         

        [HttpGet("all-HairStyles")]
        public async Task<IActionResult> GetHairStyles()
        {
            var response = await _hairStyleServices.GetAllHairStyles();
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }





        [HttpGet("all-African")]
        public async Task<IActionResult> GetAllAfricanHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllAfricanHairStyles());
        }


        [HttpGet("all-American")]
        public async Task<IActionResult> GetAllAmricanHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllAmericanHairStyles());
        }



        [HttpGet("all-European")]
        public async Task<IActionResult> GetAllEuropeanHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllAfricanHairStyles());
        }


        [HttpGet("all-Asian")]
        public async Task<IActionResult> GetAllAsianHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllAsianHairStyles());
        }


        [HttpPost("addHairStylePhoto")]
        public async Task<IActionResult> AddPhoto([FromForm] UpdateHairStylePhotoDto updatePhotoDto)
        {
            return Ok(await _hairStyleServices.AddHairStylePhoto(updatePhotoDto));
        }



        [HttpGet("getHairStylePhoto")]
        public async Task<IActionResult> ViewPhoto(string photoId)
        {
            var photoResponse = await _hairStyleServices.GetHairStylePhotoAsync(photoId);

            if (photoResponse.Succeeded)
            {
                return Ok(photoResponse);
            }
            else
            {
                return BadRequest(photoResponse);
            }
        }




        [HttpDelete("deleteHairStylePhoto")]
        public async Task<IActionResult> DeletePhoto(string photoId)
        {
            return Ok(await _hairStyleServices.DeleteHairStylePhotoAsync(photoId));
        }





    }
}
