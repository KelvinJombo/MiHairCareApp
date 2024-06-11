using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MiHairCareApp.Domain.Entities.Helper;
using MiHairCareApp.Application.Interfaces;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class StylistAuthServices : IStylistAuthServices
    {
        private readonly IEmailServices _emailServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletServices _walletServices;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<StylistAuthServices> _logger;

        public StylistAuthServices(IEmailServices emailServices, IUnitOfWork unitOfWork, IWalletServices walletServices, IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<StylistAuthServices> logger)
        {
            _emailServices = emailServices;
            _unitOfWork = unitOfWork;
            _walletServices = walletServices;
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        public async Task<ApiResponse<StylistsRegResponseDto>> RegisterAsync(CreateStylistsDto createStylistsDto)
        {
            var stylist = await _userManager.FindByEmailAsync(createStylistsDto.Email);
            if (stylist != null)
            {
                return ApiResponse<StylistsRegResponseDto>.Failed("Stylist with this email already exists.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var stylists = await _unitOfWork.UserRepository.FindAsync(x => x.PhoneNumber == createStylistsDto.PhoneNumber);
            if (stylists.Count > 0)
            {
                return ApiResponse<StylistsRegResponseDto>.Failed("Stylist with this phone number already exists.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var stylis = new AppUser()
            {
                FirstName = createStylistsDto.StylistName,
                CompanyName = createStylistsDto.CompanyName,
                Email = createStylistsDto.Email,
                PhoneNumber = createStylistsDto.PhoneNumber,
                UserName = createStylistsDto.Email,
                Town = createStylistsDto.Town,
                Street = createStylistsDto.Street,
                HomeService = createStylistsDto.HomeService,
                PasswordResetToken = ""
            };

            try
            {
                var token = "";

                var result = await _userManager.CreateAsync(stylis, createStylistsDto.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(stylis, "Admin");
                    token = await _userManager.GenerateEmailConfirmationTokenAsync(stylis);


                    var createWalletDto = new CreateWalletDto
                    {
                        PhoneNumber = stylis.PhoneNumber,
                        UserId = stylis.Id
                    };

                    var walletCreated = await _walletServices.CreateWallet(createWalletDto);
                    if (walletCreated.Succeeded)
                    {
                        var response = new StylistsRegResponseDto
                        {
                            //Id = stylis.Id,
                            Email = stylis.Email,
                            PhoneNumber = stylis.PhoneNumber,
                            StylistName = stylis.FirstName,
                            UserName = stylis.UserName,
                            CompanyName = stylis.CompanyName,
                            Token = token
                        };



                        return ApiResponse<StylistsRegResponseDto>.Success(response, "Stylist registered successfully. Please click on the link sent to your email to confirm your account", StatusCodes.Status201Created);
                    }
                    else
                    {
                        _unitOfWork.UserRepository.DeleteAsync(stylis);
                        return ApiResponse<StylistsRegResponseDto>.Failed(walletCreated.Message, StatusCodes.Status201Created, new List<string>());
                    }
                }
                else
                {
                    return ApiResponse<StylistsRegResponseDto>.Failed("Error occurred: Failed to create wallet", StatusCodes.Status201Created, new List<string>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a manager " + ex.InnerException);
                return ApiResponse<StylistsRegResponseDto>.Failed("Error creating stylist.", StatusCodes.Status500InternalServerError, new List<string>() { ex.InnerException.ToString() });
            }
        }

        public async Task<ApiResponse<StylistsLoginResponseDto>> LoginAsync(StylistsLoginDto loginDTO)
        {
            try
            {
                var stylist = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (stylist == null)
                {
                    return ApiResponse<StylistsLoginResponseDto>.Failed("Stylist not found.", StatusCodes.Status404NotFound, new List<string>());
                }
                if (!stylist.EmailConfirmed)
                {
                    return ApiResponse<StylistsLoginResponseDto>.Failed("Email not confirmed.", StatusCodes.Status401Unauthorized, new List<string>());
                }
                var result = await _signInManager.CheckPasswordSignInAsync(stylist, loginDTO.Password, lockoutOnFailure: false);

                switch (result)
                {
                    case { Succeeded: true }:
                        var role = (await _userManager.GetRolesAsync(stylist)).First();
                        stylist.LastLogin = DateTime.Now;
                        _unitOfWork.UserRepository.Update(stylist);
                        await _unitOfWork.SaveChangesAsync();
                        var response = new StylistsLoginResponseDto
                        {
                            JWToken = GenerateJwtToken(stylist, role)
                        };
                        return ApiResponse<StylistsLoginResponseDto>.Success(response, "Logged In Successfully", StatusCodes.Status200OK);

                    case { IsLockedOut: true }:
                        return ApiResponse<StylistsLoginResponseDto>.Failed($"Account is locked out. Please try again later or contact support." +
                            $" You can unlock your account after {_userManager.Options.Lockout.DefaultLockoutTimeSpan.TotalMinutes} minutes.", StatusCodes.Status403Forbidden, new List<string>());

                    case { RequiresTwoFactor: true }:
                        return ApiResponse<StylistsLoginResponseDto>.Failed("Two-factor authentication is required.", StatusCodes.Status401Unauthorized, new List<string>());

                    case { IsNotAllowed: true }:
                        return ApiResponse<StylistsLoginResponseDto>.Failed("Login failed. Email confirmation is required.", StatusCodes.Status401Unauthorized, new List<string>());

                    default:
                        return ApiResponse<StylistsLoginResponseDto>.Failed("Login failed. Invalid email or password.", StatusCodes.Status401Unauthorized, new List<string>());
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<StylistsLoginResponseDto>.Failed("Some error occurred while login in." + ex.Message, StatusCodes.Status500InternalServerError, new List<string>() { ex.Message });
            }
        }
        private string GenerateJwtToken(AppUser contact, string roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings:Secret").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, contact.Id),
                new Claim(JwtRegisteredClaimNames.Email, contact.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, contact.FirstName+" "+contact.CompanyName),
                new Claim(ClaimTypes.Role, roles)
            };

            var token = new JwtSecurityToken(
                issuer: _config.GetValue<string>("JwtSettings:ValidIssuer"),
                audience: _config.GetValue<string>("JwtSettings:ValidAudience"),
                //issuer: null,
                //audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config.GetSection("JwtSettings:AccessTokenExpiration").Value)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            try
            {
                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (result.Succeeded)
                {
                    return new ApiResponse<string>(true, "Password changed successfully.", 200, null, new List<string>());
                }
                else
                {
                    return new ApiResponse<string>(false, "Password change failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred while changing password");
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(true, "Error occurred while changing password", 500, null, errorList);
            }
        }
        public async Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "User not found.", 404, null, new List<string>());
                }

                // Additional token validation logic can be added here
                if (token.Contains(" "))
                {
                    token = token.Replace(" ", "+");
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    // Update user properties if needed
                    user.PasswordResetToken = null;
                    user.ResetTokenExpires = null;
                    await _userManager.UpdateAsync(user);

                    return new ApiResponse<string>(true, "Password reset successful.", 200, null, new List<string>());
                }
                else
                {
                    return new ApiResponse<string>(false, "Password reset failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting password for user with email {Email}", email);
                var errorList = new List<string> { "An unexpected error occurred while resetting the password." };
                return new ApiResponse<string>(true, "Error occurred while resetting password", 500, null, errorList);
            }
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
        {
            try
            {
                var stylist = await _userManager.FindByEmailAsync(email);

                if (stylist == null)
                {
                    return new ApiResponse<string>(false, "Email does not exist", StatusCodes.Status404NotFound, null, new List<string>());
                }

                if (!stylist.EmailConfirmed)
                {
                    return new ApiResponse<string>(false, "Stylist's email not confirmed.", StatusCodes.Status404NotFound, null, new List<string>());
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(stylist);

                //  token = HttpUtility.UrlEncode(token);

                stylist.PasswordResetToken = token;
                stylist.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

                await _userManager.UpdateAsync(stylist);

                var resetPasswordUrl = "http://localhost:3000/confirmpassword?email=" + email + "&token=" + token;


                var mailRequest = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Your Savi Password Reset Instructions",
                    Body = $"Please reset your password by clicking <a href='{resetPasswordUrl}'>here</a>."
                };
                await _emailServices.SendMailAsync(mailRequest);

                return new ApiResponse<string>(true, "Password reset email sent successfully.", 200, null, new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing forgot password for user with email {Email}", email);
                var errorList = new List<string> { "An unexpected error occurred while processing the forgot password request." };
                return new ApiResponse<string>(true, "Error occurred while processing forgot password", 500, null, errorList);
            }
        }

        public async Task<ApiResponse<string>> ConfirmEmail(string userid, string token)
        {
            if (userid == null || token == null)
            {
                return ApiResponse<string>.Failed("Stylist id or token should not be null", StatusCodes.Status400BadRequest, new List<string>() { });
            }

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return ApiResponse<string>.Failed("Stylist instance is Invalid", StatusCodes.Status404NotFound, new List<string>() { });
            }
            token = token.Replace(" ", "+");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return ApiResponse<string>.Success("success", "Email confirmed successfully", StatusCodes.Status200OK);
            }
            else
            {
                return ApiResponse<string>.Failed("Failed. " + result.Errors.ToArray()[0].Description, StatusCodes.Status500InternalServerError, new List<string>() { });
            }
        }



        public ApiResponse<string> ExtractUserIdFromToken(string authToken)
        {
            try
            {
                var token = authToken.Replace("Bearer ", "");

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                var userId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return new ApiResponse<string>(false, "Invalid or expired token.", 401, null, new List<string>());
                }

                return new ApiResponse<string>(true, "User ID extracted successfully.", 200, userId, new List<string>());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Error extracting user ID from token.", 500, null, new List<string> { ex.Message });
            }
        }

    }
}
