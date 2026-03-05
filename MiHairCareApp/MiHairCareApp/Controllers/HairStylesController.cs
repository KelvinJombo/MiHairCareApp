using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Enums;

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
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                return BadRequest(new { Message = "Model binding failed", Errors = errors });
            }

            if (!Enum.TryParse<HairStyleOrigin>(dto.Origin, true, out var originEnum))
            {
                return BadRequest($"Invalid origin value: {dto.Origin}");
            }

            dto.OriginEnum = originEnum;

            var result = await _hairStyleServices.AddHairStyleAsync(dto);

            if (!result.Succeeded)
                return StatusCode(result.StatusCode, result);

            return Ok(result);
        }






        //[Authorize(Roles = "Admin")]
        [HttpGet("getById")]
        public async Task<IActionResult> GetHairStyleById(string hairStyleId)
        {
            return Ok(await _hairStyleServices.GetHairStyleById(hairStyleId));
        }


        [HttpGet("getByTitle")]
        public async Task<IActionResult> GetHairStyleByName(string hairStyleTitle)
        {
            return Ok(await _hairStyleServices.GetHairStyleByTitle(hairStyleTitle));
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

        [HttpGet("stylesForPortfolio")]
        public async Task<IActionResult> GetStylesForPortfolio()
        {
            var response = await _hairStyleServices.GetAllPortFolioStyles();
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
        [HttpDelete("deleteHairStyle")]
        public async Task<IActionResult> DeleteHairStyle(string hairStyleId)
        {
            return Ok(await _hairStyleServices.DeleteAHairStyle(hairStyleId));
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

