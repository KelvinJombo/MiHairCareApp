using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailServices _emailServices;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthenticationController(IAuthenticationServices authenticationService,UserManager<AppUser> userManager, IEmailServices emailServices, SignInManager<AppUser> signInManager)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
            _emailServices = emailServices;
            _signInManager = signInManager;
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



        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            return Ok(await _authenticationService.ConfirmEmail(userid, token));
        }


        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            var response = await _authenticationService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

            if (response.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
            }

             
        }



        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto model, [FromHeader(Name = "Authorization")] string authToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            if (string.IsNullOrWhiteSpace(authToken))
            {
                return Unauthorized(new ApiResponse<string>(false, "Authorization token is missing.", 401, null, new List<string>()));
            }

            var userIdResponse = _authenticationService.ExtractUserIdFromToken(authToken);

            if (!userIdResponse.Succeeded)
            {
                return Unauthorized(userIdResponse);
            }
            var userId = userIdResponse.Data;

            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return Unauthorized(new ApiResponse<string>(false, "User not found.", 401, null, new List<string>()));
            }

            var response = await _authenticationService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (response.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
            }
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return Ok(new ApiResponse<string>(true, "Logout successful", 200, null, new List<string>()));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            var response = await _authenticationService.ForgotPasswordAsync(model.Email);

            if (response.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
            }
        }


    }
}
