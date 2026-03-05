using AutoMapper;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Entities.Helper;
using MiHairCareApp.Domain.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;
        private readonly string _clientUrl;

        public StylistAuthServices(IEmailServices emailServices, IUnitOfWork unitOfWork, IWalletServices walletServices, IConfiguration config, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<StylistAuthServices> logger, IJwtTokenService jwtTokenService, IMapper mapper)
        {
            _emailServices = emailServices;
            _unitOfWork = unitOfWork;
            _walletServices = walletServices;
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
            _clientUrl = config["AppSettings:ClientUrl"] ?? "";
        }

        public async Task<ApiResponse<StylistsRegResponseDto>> RegisterAsync(CreateStylistsDto createStylistsDto)
        {
            if (createStylistsDto == null) throw new ValidationException("Payload cannot be null");

            if (await _userManager.FindByEmailAsync(createStylistsDto.Email) is not null)
                throw new ValidationException("Stylist with this email already exists. Try the Reset Password Route");

            var phoneExists = await _unitOfWork.UserRepository.FindAsync(x => x.PhoneNumber == createStylistsDto.PhoneNumber);
            if (phoneExists.Any())
                throw new ValidationException("Stylist with this phone number already exists. Try the Reset Password Route");

            var stylist = new AppUser
            {
                FirstName = createStylistsDto.StylistName,
                CompanyName = createStylistsDto.CompanyName,
                Email = createStylistsDto.Email,
                PhoneNumber = createStylistsDto.PhoneNumber,
                UserName = createStylistsDto.Email,
                Town = createStylistsDto.Town,
                Street = createStylistsDto.Street,
                HomeService = createStylistsDto.HomeService,
                PasswordResetToken = string.Empty
            };

            var result = await _userManager.CreateAsync(stylist, createStylistsDto.Password);
            if (!result.Succeeded)
                return ApiResponse<StylistsRegResponseDto>.Failed("Failed to create stylist account.", StatusCodes.Status400BadRequest, result.Errors.Select(e => e.Description).ToList());

            await _userManager.AddToRoleAsync(stylist, "Admin");

            var walletCreated = await _walletServices.CreateWallet(new CreateWalletDto
            {
                PhoneNumber = stylist.PhoneNumber,
                UserId = stylist.Id
            });

            if (walletCreated == null)
            {
                await _userManager.DeleteAsync(stylist);
                throw new ServiceException("Wallet creation failed. Stylist registration rolled back.");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(stylist);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = $"{_clientUrl}/confirm-email?userId={stylist.Id}&token={encodedToken}";

            var emailSent = await _emailServices.SendMailAsync(new MailRequest
            {
                ToEmail = stylist.Email!,
                Subject = "Confirm your email - MiHairCare App",
                Body = $"Hi {stylist.FirstName}, <br/> Please confirm your account by clicking <a href='{confirmationLink}'>here</a>."
            });

            if (!emailSent)
            {
                await _userManager.DeleteAsync(stylist);
                throw new ServiceException("Stylist created, but confirmation email could not be sent. Registration rolled back.");
            }

            var response = new StylistsRegResponseDto
            {
                Email = stylist.Email,
                PhoneNumber = stylist.PhoneNumber,
                StylistName = stylist.FirstName,
                UserName = stylist.UserName,
                CompanyName = stylist.CompanyName
            };

            return ApiResponse<StylistsRegResponseDto>.Success(response, "Stylist registered successfully. Please check your email to confirm your account.", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<StylistsRegResponseDto>> RegisterWithGoogleAsync(string idToken, string phoneNumber)
        {
            if (string.IsNullOrEmpty(idToken)) throw new ValidationException("Google has not authenticated this user");

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
            var email = payload.Email;
            var name = payload.Name ?? payload.GivenName ?? "Unknown";

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                throw new ValidationException("User already registered. Please log in instead.");

            var newUser = new AppUser
            {
                FirstName = name,
                Email = email,
                UserName = email,
                PhoneNumber = phoneNumber,
                EmailConfirmed = true,
                PasswordResetToken = string.Empty,
                CompanyName = "N/A",
                Town = "N/A",
                Street = "N/A",
                HomeService = false
            };

            var createUserResult = await _userManager.CreateAsync(newUser);
            if (!createUserResult.Succeeded)
                return ApiResponse<StylistsRegResponseDto>.Failed("Failed to create user.", StatusCodes.Status400BadRequest, createUserResult.Errors.Select(e => e.Description).ToList());

            var walletDto = new CreateWalletDto { UserId = newUser.Id, PhoneNumber = phoneNumber };
            var walletResponse = await _walletServices.CreateWallet(walletDto);
            if (!walletResponse.Succeeded)
                throw new ServiceException("Failed to create wallet for user.");

            await _signInManager.SignInAsync(newUser, isPersistent: false);

            var response = new StylistsRegResponseDto
            {
                Email = newUser.Email,
                PhoneNumber = newUser.PhoneNumber,
                StylistName = newUser.FirstName,
                UserName = newUser.UserName,
                CompanyName = newUser.CompanyName
            };

            return ApiResponse<StylistsRegResponseDto>.Success(response, "User registered successfully with Google and wallet created.", StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<LoginResponseDto>> VerifyAndAuthenticateUserAsync(string idToken)
        {
            if (string.IsNullOrWhiteSpace(idToken)) throw new ValidationException("ID token is required");

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
            var userEmail = payload.Email;

            var existingUser = await _userManager.FindByEmailAsync(userEmail);
            if (existingUser == null)
                throw new NotFoundException("Email not registered");

            await _signInManager.SignInAsync(existingUser, isPersistent: false);

            var role = (await _userManager.GetRolesAsync(existingUser)).FirstOrDefault() ?? "User";
            var response = new LoginResponseDto
            {
                Token = _jwtTokenService.GenerateToken(existingUser.Id, existingUser.Email!, role),
                UserId = existingUser.Id,
                Email = existingUser.Email
            };

            return ApiResponse<LoginResponseDto>.Success(response, "User logged in successfully", StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<string>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:Secret").Value!);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config.GetSection("JwtSettings:ValidIssuer").Value,
                    ValidAudience = _config.GetSection("JwtSettings:ValidAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                return new ApiResponse<string>(true, "Token is valid.", 200, null, new List<string>());
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Token validation failed");
                return new ApiResponse<string>(false, "Token validation failed.", 400, null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(StylistsLoginDto loginDTO)
        {
            if (loginDTO == null) throw new ValidationException("Login payload is required");

            var stylist = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (stylist == null) throw new NotFoundException("Stylist not found.");

            var result = await _signInManager.CheckPasswordSignInAsync(stylist, loginDTO.Password, lockoutOnFailure: false);
            switch (result)
            {
                case { Succeeded: true }:
                    var role = (await _userManager.GetRolesAsync(stylist)).First();
                    stylist.LastLogin = DateTime.Now;
                    _unitOfWork.UserRepository.Update(stylist);
                    await _unitOfWork.SaveChangesAsync();
                    var response = new LoginResponseDto
                    {
                        Token = _jwtTokenService.GenerateToken(stylist.Id, stylist.Email!, role),
                        UserId = stylist.Id,
                        Email = loginDTO.Email,
                        FirstName = stylist.FirstName,
                    };
                    return ApiResponse<LoginResponseDto>.Success(response, "Logged In Successfully", StatusCodes.Status200OK);

                case { IsLockedOut: true }:
                    return ApiResponse<LoginResponseDto>.Failed($"Account is locked out.", StatusCodes.Status403Forbidden, new List<string>());

                default:
                    return ApiResponse<LoginResponseDto>.Failed("Login failed. Invalid email or password.", StatusCodes.Status401Unauthorized, new List<string>());
            }
        }

        public async Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            if (user == null) throw new ValidationException("User is required");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (result.Succeeded) return new ApiResponse<string>(true, "Password changed successfully.", 200, null, new List<string>());

            return new ApiResponse<string>(false, "Password change failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
                throw new ValidationException("Invalid input");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new NotFoundException("User not found.");

            if (token.Contains(" ")) token = token.Replace(" ", "+");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
            {
                user.PasswordResetToken = null;
                user.ResetTokenExpires = null;
                await _userManager.UpdateAsync(user);
                return new ApiResponse<string>(true, "Password reset successful.", 200, null, new List<string>());
            }

            return new ApiResponse<string>(false, "Password reset failed.", 400, null, result.Errors.Select(error => error.Description).ToList());
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ValidationException("Email is required");

            var stylist = await _userManager.FindByEmailAsync(email);
            if (stylist == null) throw new NotFoundException("Email does not exist");

            if (!stylist.EmailConfirmed) throw new ValidationException("Stylist's email not confirmed.");

            string token = await _userManager.GeneratePasswordResetTokenAsync(stylist);
            stylist.PasswordResetToken = token;
            stylist.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _userManager.UpdateAsync(stylist);

            return new ApiResponse<string>(true, "Password reset token retrieved successfully.", 200, token, new List<string>());
        }

        public async Task<ApiResponse<string>> ConfirmEmail(string userid, string token)
        {
            if (userid == null || token == null) throw new ValidationException("Stylist id or token should not be null");

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null) throw new NotFoundException("Stylist instance is Invalid");

            token = token.Replace(" ", "+");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return ApiResponse<string>.Success("success", "Email confirmed successfully", StatusCodes.Status200OK);

            return ApiResponse<string>.Failed("Failed. " + result.Errors.ToArray()[0].Description, StatusCodes.Status500InternalServerError, new List<string>() { });
        }

        public ApiResponse<string> ExtractUserIdFromToken(string authToken)
        {
            try
            {
                var token = authToken.Replace("Bearer ", "");
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                var userId = jsonToken?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrWhiteSpace(userId)) return new ApiResponse<string>(false, "Invalid or expired token.", 401, null, new List<string>());

                return new ApiResponse<string>(true, "User ID extracted successfully.", 200, userId, new List<string>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting user id");
                return new ApiResponse<string>(false, "Error extracting user ID from token.", 500, null, new List<string> { ex.Message });
            }
        }

        public async Task<bool> UpdateStylistPortfolioAsync(string userId, List<string> hairStyleIds)
        {
            var user = await _unitOfWork.UserRepository.Query(u => u.Id == userId)
                .Include(u => u.StylistPortfolio)
                .ThenInclude(p => p.HairStyles)
                .FirstOrDefaultAsync();

            if (user == null) return false;

            if (user.StylistPortfolio == null)
            {
                user.StylistPortfolio = new StylistPortfolio { UserID = userId, HairStyles = new List<HairStyle>() };
            }

            var selectedHairStyles = await _unitOfWork.HairStyleRepository.Query(h => hairStyleIds.Contains(h.Id)).ToListAsync();
            user.StylistPortfolio.HairStyles = selectedHairStyles;
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<ApiResponse<List<RegisterResponseDto>>> GetStylistUsers()
        {
            var users = await _unitOfWork.UserRepository.FindAsync(user => user.CompanyName != null);
            var result = _mapper.Map<List<RegisterResponseDto>>(users);
            return ApiResponse<List<RegisterResponseDto>>.Success(result, "Users retrieved successfully", 200);
        }


        public async Task<ApiResponse<List<PortfolioResponseDto>>> GetStylistsByHairStyle(string hairStyleId)
        {
            var stylists = await _unitOfWork.UserRepository
                .Query()
                .Where(u => u.StylistPortfolio != null &&
                            u.StylistPortfolio.HairStyles.Any(h => h.Id == hairStyleId))
                .Include(u => u.StylistPortfolio)
                    .ThenInclude(p => p.HairStyles)
                .ToListAsync();

            if (!stylists.Any())
            {
                return ApiResponse<List<PortfolioResponseDto>>.Failed(
                    "No stylists found for this hairstyle",
                    404,
                    new List<string>()
                );
            }

            var result = _mapper.Map<List<PortfolioResponseDto>>(stylists);

            return ApiResponse<List<PortfolioResponseDto>>.Success(
                result,
                "Stylists retrieved successfully",
                200
            );
        }


    }
}
