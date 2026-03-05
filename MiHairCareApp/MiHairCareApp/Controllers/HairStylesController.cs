using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Enums;
using MiHairCareApp.Extensions;

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
        public async Task<IActionResult> AddHairStyle([FromForm] CreateHairStyleDto dto)
        {
            var result = await _hairStyleServices.AddHairStyleAsync(dto);
            return result.ToActionResult();
            
        }


        //[Authorize(Roles = "Admin")]
        [HttpGet("getById")]
        public async Task<IActionResult> GetHairStyleById(string hairStyleId)
        {
            var result = await _hairStyleServices.GetHairStyleById(hairStyleId);
            return result.ToActionResult();
        }


        [HttpGet("getByTitle")]
        public async Task<IActionResult> GetHairStyleByName(string hairStyleTitle)
        {
            var result = await _hairStyleServices.GetHairStyleByTitle(hairStyleTitle);
            return result.ToActionResult();
        }




        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-HairStyles")]
        public async Task<IActionResult> GetHairStyles()
        {
            var response = await _hairStyleServices.GetAllHairStyles();
            return response.ToActionResult();
            
        }

        //[HttpGet("stylesForPortfolio")]
        //public async Task<IActionResult> GetStylesForPortfolio()
        //{
        //    var response = await _hairStyleServices.GetStylistPortFolioAsync();
        //    if (response != null)
        //    {
        //        return Ok(response);
        //    }
        //    return BadRequest(response);
        //} 

        [HttpGet("{userId}/portfolio")]
        public async Task<IActionResult> GetStylistPortfolio(string userId)
        {
            var result = await _hairStyleServices.GetStylistPortfolioAsync(userId);
            return result.ToActionResult();
        }


        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-African")]
        public async Task<IActionResult> GetAllAfricanHairStyles()
        {
            var result = await _hairStyleServices.GetAllAfricanHairStyles();

            return result.ToActionResult();
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-American")]
        public async Task<IActionResult> GetAllAmricanHairStyles()
        {
            var result = await _hairStyleServices.GetAllAmericanHairStyles();
            return result.ToActionResult();
        }


        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-European")]
        public async Task<IActionResult> GetAllEuropeanHairStyles()
        {
            var result = await _hairStyleServices.GetAllEuropeanHairStyles();
            return result.ToActionResult();
        }

        //[Authorize(Roles = "Admin, User")]
        [HttpGet("all-Asian")]
        public async Task<IActionResult> GetAllAsianHairStyles()
        {
            var result = await _hairStyleServices.GetAllAsianHairStyles();
            return result.ToActionResult();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("addHairStylePhoto")]
        public async Task<IActionResult> AddPhoto([FromForm] UpdateHairStylePhotoDto updatePhotoDto)
        {
            var result = await _hairStyleServices.AddHairStylePhoto(updatePhotoDto);
            return result.ToActionResult();
        }


        //[Authorize(Roles = "Admin, User")]
        [HttpGet("getHairStylePhoto")]
        public async Task<IActionResult> ViewPhoto(string photoId)
        {
            var photoResponse = await _hairStyleServices.GetHairStylePhotoAsync(photoId);
            return photoResponse.ToActionResult();
           
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("deleteHairStyle")]
        public async Task<IActionResult> DeleteHairStyle(string hairStyleId)
        {
            var result = await _hairStyleServices.DeleteAHairStyle(hairStyleId);
            return result.ToActionResult();
        }




        //[Authorize(Roles = "Admin")]
        [HttpDelete("deleteHairStylePhoto")]
        public async Task<IActionResult> DeletePhoto(string photoId)
        {
            var result = await _hairStyleServices.DeleteHairStylePhotoAsync(photoId);
            return result ? Ok(new ApiResponse<bool>(true, "Photo deleted successfully", 200, true, new List<string>()))
                          : BadRequest(new ApiResponse<bool>(false, "Failed to delete photo", 400, false, new List<string>()));
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

