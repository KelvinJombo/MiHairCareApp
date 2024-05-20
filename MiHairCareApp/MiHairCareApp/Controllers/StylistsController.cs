using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StylistsController : ControllerBase
    {
        private readonly IStylistServices _stylistServices;
        private readonly IEmailServices _emailServices;

        public StylistsController(IStylistServices stylistServices, IEmailServices emailServices)
        {
            _stylistServices = stylistServices;
            _emailServices = emailServices;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateStylistsDto createStylistsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.Failed("Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            // Call registration service
            var registrationResult = await _stylistServices.RegisterAsync(createStylistsDto);



            if (registrationResult.Succeeded)
            {
                var data = registrationResult.Data;
                //  _backgroundJobClient.Enqueue(() => Console.WriteLine(data));
                var confirmationLink = GenerateConfirmEmailLink(data.Id, data.Token);
                return Ok(await _emailServices.SendEmailAsync(confirmationLink, data.Email, data.Id));
            }
            else
            {
                return BadRequest(new { Message = registrationResult.Message, Errors = registrationResult.Errors });
            }
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(StylistsLoginDto loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.Failed("Invalid model state.", 400, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _stylistServices.LoginAsync(loginDTO));
        }


        private static string GenerateConfirmEmailLink(string id, string token)
        {
            return "http://localhost:3000/email-confirmed?UserId=" + id + "&token=" + token;
        }



        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail(string userid, string token)
        //{
        //    return Ok(await _authenticationService.ConfirmEmail(userid, token));
        //}

        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToList();

        //        return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
        //    }

        //    var response = await _authenticationService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

        //    if (response.Succeeded)
        //    {
        //        return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
        //    }
        //    else
        //    {
        //        return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
        //    }


        //}
    }
}
