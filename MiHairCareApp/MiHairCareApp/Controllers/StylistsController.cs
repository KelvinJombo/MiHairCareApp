using Microsoft.AspNetCore.Authentication;
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
    public class StylistsController : ControllerBase
    {
        private readonly IStylistAuthServices _stylistServices;
        private readonly IEmailServices _emailServices;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public StylistsController(IStylistAuthServices stylistServices, IEmailServices emailServices, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _stylistServices = stylistServices;
            _emailServices = emailServices;
            _userManager = userManager;
            _signInManager = signInManager;
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

        /// <summary>
        /// Register a new user using Google Authentication
        /// </summary>
        [HttpPost("google-register")]
        [ProducesResponseType(typeof(ApiResponse<StylistsRegResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<StylistsRegResponseDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<StylistsRegResponseDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterWithGoogle([FromBody] GoogleRegisterRequestDto model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.IdToken))
                return BadRequest(ApiResponse<StylistsRegResponseDto>.Failed(
                    "Invalid request data",
                    StatusCodes.Status400BadRequest,
                    new List<string> { "Google ID token is required" }
                ));

            try
            {
                var result = await _stylistServices.RegisterWithGoogleAsync(model.IdToken, model.PhoneNumber);

                if (result.Succeeded)
                    return StatusCode(StatusCodes.Status201Created, result);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                 
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<StylistsRegResponseDto>.Failed(
                        "An unexpected error occurred during Google registration",
                        StatusCodes.Status500InternalServerError,
                        new List<string> { ex.Message }
                    ));
            }
        }



        [HttpPost("login-google/{token}")]
        public async Task<IActionResult> GoogleAuth([FromRoute] string token)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", StatusCodes.Status400BadRequest, "", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }
            return Ok(await _stylistServices.VerifyAndAuthenticateUserAsync(token));
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




        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model state.", 400, null, ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList()));
            }

            var response = await _stylistServices.ForgotPasswordAsync(model.Email);

            if (response.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, response.Data, new List<string>()));
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

            var userIdResponse = _stylistServices.ExtractUserIdFromToken(authToken);

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

            var response = await _stylistServices.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (response.Succeeded)
            {
                return Ok(new ApiResponse<string>(true, response.Message, response.StatusCode, null, new List<string>()));
            }
            else
            {
                return BadRequest(new ApiResponse<string>(false, response.Message, response.StatusCode, null, response.Errors));
            }
        }




        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userid, string token)
        {
            return Ok(await _stylistServices.ConfirmEmail(userid, token));
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

            var response = await _stylistServices.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

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
               



    }
}
