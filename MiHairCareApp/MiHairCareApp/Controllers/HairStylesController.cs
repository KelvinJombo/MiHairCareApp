using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;

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


        //[Authorize(Roles = "Admin")]
        [HttpPost("addHairStyle")]
        public async Task<IActionResult> AddHairStyle(CreateHairStyleDto createHairStyleDto)
        {
            return Ok(await _hairStyleServices.AddHairStyleAsync(createHairStyleDto));
        }




        //[Authorize(Roles = "Admin")]
        [HttpGet("getById")]
        public async Task<IActionResult> GetHairStyleById(string hairStyleId)
        {
            return Ok(await _hairStyleServices.GetHairStyleById(hairStyleId));
        }



        //[Authorize(Roles = "Admin, User")]
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



        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-African")]
        public async Task<IActionResult> GetAllAfricanHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllAfricanHairStyles());
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-American")]
        public async Task<IActionResult> GetAllAmricanHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllAmericanHairStyles());
        }


        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-European")]
        public async Task<IActionResult> GetAllEuropeanHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllEuropianHairStyles());
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-Asian")]
        public async Task<IActionResult> GetAllAsianHairStyles()
        {
            return Ok(await _hairStyleServices.GetAllAsianHairStyles());
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("addHairStylePhoto")]
        public async Task<IActionResult> AddPhoto([FromForm] UpdateHairStylePhotoDto updatePhotoDto)
        {
            return Ok(await _hairStyleServices.AddHairStylePhoto(updatePhotoDto));
        }


        //[Authorize(Roles = "Admin, User")]
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



        //[Authorize(Roles = "Admin")]
        [HttpDelete("deleteHairStylePhoto")]
        public async Task<IActionResult> DeletePhoto(string photoId)
        {
            return Ok(await _hairStyleServices.DeleteHairStylePhotoAsync(photoId));
        }


        //[Authorize(Roles = "Admin")]
        [HttpPut("update")]
        [ProducesResponseType(typeof(ApiResponse<HairStyleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<HairStyleResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<HairStyleResponseDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateHairStyle([FromForm] UpdateHairStylePhotoDto updatePhotoDto)
        {
            var response = await _hairStyleServices.UpdateHairStyleAsync(updatePhotoDto);
            return StatusCode(response.StatusCode, response);
        }
    }


}

