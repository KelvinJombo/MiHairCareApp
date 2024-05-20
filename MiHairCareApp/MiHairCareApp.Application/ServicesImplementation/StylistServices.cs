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
using MiHairCareApp.Application.Interfaces;

namespace MiHairCareApp.Application.ServicesImplementation
{
    public class StylistServices : IStylistServices
    {
        private readonly IEmailServices _emailServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWalletServices _walletServices;
        private readonly IConfiguration _config;
        private readonly UserManager<Stylist> _userManager;
        private readonly SignInManager<Stylist> _signInManager;
        private readonly ILogger<StylistServices> _logger;

        public StylistServices(IEmailServices emailServices, IUnitOfWork unitOfWork, IWalletServices walletServices, IConfiguration config, UserManager<Stylist> userManager, SignInManager<Stylist> signInManager, ILogger<StylistServices> logger)
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
                return ApiResponse<StylistsRegResponseDto>.Failed("User with this email already exists.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var stylists = await _unitOfWork.StylistRepository.FindAsync(x => x.PhoneNumber == createStylistsDto.PhoneNumber);
            if (stylists.Count > 0)
            {
                return ApiResponse<StylistsRegResponseDto>.Failed("User with this phone number already exists.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var stylis = new Stylist()
            {
                StylistName = createStylistsDto.StylistName,
                CompanyName = createStylistsDto.CompanyName,
                Email = createStylistsDto.Email,
                PhoneNumber = createStylistsDto.PhoneNumber,
                //UserName = createStylistsDto.Email,
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
                            Id = stylis.Id,
                            Email = stylis.Email,
                            PhoneNumber = stylis.PhoneNumber,
                            StylistName = stylis.StylistName,
                            CompanyName = stylis.CompanyName,
                            Token = token
                        };



                        return ApiResponse<StylistsRegResponseDto>.Success(response, "User registered successfully. Please click on the link sent to your email to confirm your account", StatusCodes.Status201Created);
                    }
                    else
                    {
                        _unitOfWork.StylistRepository.DeleteAsync(stylis);
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
                return ApiResponse<StylistsRegResponseDto>.Failed("Error creating user.", StatusCodes.Status500InternalServerError, new List<string>() { ex.InnerException.ToString() });
            }
        }

        public async Task<ApiResponse<StylistsLoginResponseDto>> LoginAsync(StylistsLoginDto loginDTO)
        {
            try
            {
                var stylist = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (stylist == null)
                {
                    return ApiResponse<StylistsLoginResponseDto>.Failed("User not found.", StatusCodes.Status404NotFound, new List<string>());
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
                        _unitOfWork.StylistRepository.Update(stylist);
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
        private string GenerateJwtToken(Stylist contact, string roles)
        {
            var jwtSettings = _config.GetSection("JwtSettings:Secret").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, contact.Id),
                new Claim(JwtRegisteredClaimNames.Email, contact.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, contact.StylistName+" "+contact.CompanyName),
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



    }
}
