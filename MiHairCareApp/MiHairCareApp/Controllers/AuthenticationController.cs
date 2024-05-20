using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationService;
        private readonly IEmailServices _emailServices;

        public AuthenticationController(IAuthenticationServices authenticationService, IEmailServices emailServices)
        {
            _authenticationService = authenticationService;
            _emailServices = emailServices;
        }

        [HttpPost("Register")] 
        public async Task<IActionResult> Register([FromBody]UserCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.Failed("Invalid model state.", StatusCodes.Status400BadRequest, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            // Call registration service
            var registrationResult = await _authenticationService.RegisterAsync(createDto);


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



        private static string GenerateConfirmEmailLink(string id, string token)
        {
            return "https://localhost:7261/email-confirmed?UserId=" + id + "&token=" + token;
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(AppUserLoginDto loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<string>.Failed("Invalid model state.", 400, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _authenticationService.LoginAsync(loginDTO));
        }


    }
}
