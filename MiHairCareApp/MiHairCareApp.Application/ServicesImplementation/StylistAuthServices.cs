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
            try
            {
                // 🔹 Check if email already exists
                if (await _userManager.FindByEmailAsync(createStylistsDto.Email) is not null)
                {
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "Stylist with this email already exists. Try the Reset Password Route",
                        StatusCodes.Status400BadRequest, new List<string>());
                }


                var phoneExists = await _unitOfWork.UserRepository.FindAsync(x => x.PhoneNumber == createStylistsDto.PhoneNumber);
                if (phoneExists.Any())
                {
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "Stylist with this phone number already exists. Try the Reset Password Route",
                        StatusCodes.Status400BadRequest, new List<string>());
                }

                // 🔹 Create new stylist entity
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

                // 🔹 Save user
                var result = await _userManager.CreateAsync(stylist, createStylistsDto.Password);
                if (!result.Succeeded)
                {
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "Failed to create stylist account.",
                        StatusCodes.Status400BadRequest,
                        result.Errors.Select(e => e.Description).ToList());
                }

                // 🔹 Assign role
                await _userManager.AddToRoleAsync(stylist, "Admin");

                // 🔹 Create wallet
                var walletCreated = await _walletServices.CreateWallet(new CreateWalletDto
                {
                    PhoneNumber = stylist.PhoneNumber,
                    UserId = stylist.Id
                });

                if (walletCreated == null)
                {
                    await _userManager.DeleteAsync(stylist); // rollback
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "Wallet creation failed. Stylist registration rolled back.",
                        StatusCodes.Status500InternalServerError, new List<string>());
                }

                // 🔹 Generate confirmation link
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(stylist);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmationLink = $"{_clientUrl}/confirm-email?userId={stylist.Id}&token={encodedToken}";

                // 🔹 Send confirmation email
                var emailSent = await _emailServices.SendMailAsync(new MailRequest
                {
                    ToEmail = stylist.Email!,
                    Subject = "Confirm your email - MiHairCare App",
                    Body = $"Hi {stylist.FirstName}, <br/> Please confirm your account by clicking <a href='{confirmationLink}'>here</a>."
                });

                if (!emailSent)
                {
                    await _userManager.DeleteAsync(stylist);  //roll back Stylist Creation
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "Stylist created, but confirmation email could not be sent. Registration rolled back.",
                        StatusCodes.Status500InternalServerError, new List<string>());
                }

                // 🔹 Build success response
                var response = new StylistsRegResponseDto
                {
                    Email = stylist.Email,
                    PhoneNumber = stylist.PhoneNumber,
                    StylistName = stylist.FirstName,
                    UserName = stylist.UserName,
                    CompanyName = stylist.CompanyName
                };

                return ApiResponse<StylistsRegResponseDto>.Success(
                    response,
                    "Stylist registered successfully. Please check your email to confirm your account.",
                    StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering stylist.");
                return ApiResponse<StylistsRegResponseDto>.Failed(
                    "An unexpected error occurred while registering stylist.",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<StylistsRegResponseDto>> RegisterWithGoogleAsync(string idToken, string phoneNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(idToken))
                    return ApiResponse<StylistsRegResponseDto>.Failed("Google has not authenticated this user", StatusCodes.Status400BadRequest, new List<string>());

                // ✅ Step 1: Validate Google Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    idToken,
                    new GoogleJsonWebSignature.ValidationSettings()
                );

                var email = payload.Email;
                var name = payload.Name ?? payload.GivenName ?? "Unknown";
                var googleId = payload.Subject;

                // ✅ Step 2: Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "User already registered. Please log in instead.",
                        StatusCodes.Status400BadRequest,
                        new List<string>()
                    );
                }

                // ✅ Step 3: Collect additional details (from frontend or Google data)
                // You can pass phoneNumber from the frontend or use `payload.PhoneNumber` if available.
                var newUser = new AppUser
                {
                    FirstName = name,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phoneNumber,
                    EmailConfirmed = true,
                    PasswordResetToken = string.Empty,
                    // Optional additional fields for stylist registration
                    CompanyName = "N/A",
                    Town = "N/A",
                    Street = "N/A",
                    HomeService = false
                };

                // ✅ Step 4: Create user in the Identity system
                var createUserResult = await _userManager.CreateAsync(newUser);
                if (!createUserResult.Succeeded)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "Failed to create user.",
                        StatusCodes.Status400BadRequest,
                        errors
                    );
                }

                // ✅ Step 5: Create Wallet for the new user
                var walletDto = new CreateWalletDto
                {
                    UserId = newUser.Id,
                    PhoneNumber = phoneNumber
                };

                var walletResponse = await _walletServices.CreateWallet(walletDto);
                if (!walletResponse.Succeeded)
                {
                    return ApiResponse<StylistsRegResponseDto>.Failed(
                        "Failed to create wallet for user.",
                        StatusCodes.Status500InternalServerError,
                        new List<string>()
                    );
                }

                // ✅ Step 6: Sign in the user
                await _signInManager.SignInAsync(newUser, isPersistent: false);

                // ✅ Step 7: Build comprehensive success response
                var response = new StylistsRegResponseDto
                {
                    Email = newUser.Email,
                    PhoneNumber = newUser.PhoneNumber,
                    StylistName = newUser.FirstName,
                    UserName = newUser.UserName,
                    CompanyName = newUser.CompanyName
                };

                return ApiResponse<StylistsRegResponseDto>.Success(
                    response,
                    "User registered successfully with Google and wallet created.",
                    StatusCodes.Status201Created
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user with Google");
                return ApiResponse<StylistsRegResponseDto>.Failed(
                    "Error occurred while registering user.",
                    StatusCodes.Status500InternalServerError,
                    new List<string> { ex.Message }
                );
            }
        }


        public async Task<ApiResponse<List<RegisterResponseDto>>> GetUsersWithNullCompanyNameAsync()
        {
            var result = await _unitOfWork.UserRepository.FindAsync(user => user.CompanyName == null);
            var results = _mapper.Map<List<RegisterResponseDto>>(result);
            return ApiResponse<List<RegisterResponseDto>>.Success(results, "Users retrieved successfully", 200);
        }

        public async Task<ApiResponse<LoginResponseDto>> VerifyAndAuthenticateUserAsync(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
                var userEmail = payload.Email;

                var existingUser = await _userManager.FindByEmailAsync(userEmail);
                if (existingUser == null)
                {
                    return ApiResponse<LoginResponseDto>.Failed("Email not registered", StatusCodes.Status404NotFound, new List<string>());
                }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while authenticating user");
                return ApiResponse<LoginResponseDto>.Failed("Error occurred while authenticating user", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
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

                var emailClaim = principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

                return new ApiResponse<string>(true, "Token is valid.", 200, null, new List<string>());
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogError(ex, "Token validation failed");
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Token validation failed.", 400, null, errorList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                var errorList = new List<string> { ex.Message };
                return new ApiResponse<string>(false, "Error occurred during token validation", 500, null, errorList);
            }
        }
       


        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(StylistsLoginDto loginDTO)
        {
            try
            {
                var stylist = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (stylist == null)
                {
                    return ApiResponse<LoginResponseDto>.Failed("Stylist not found.", StatusCodes.Status404NotFound, new List<string>());
                }
                if (!stylist.EmailConfirmed)
                {
                    stylist.EmailConfirmed = true;
                    //return ApiResponse<StylistsLoginResponseDto>.Failed("Email not confirmed.", StatusCodes.Status401Unauthorized, new List<string>());
                }
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
                        return ApiResponse<LoginResponseDto>.Failed($"Account is locked out. Please try again later or contact support." +
                            $" You can unlock your account after {_userManager.Options.Lockout.DefaultLockoutTimeSpan.TotalMinutes} minutes.", StatusCodes.Status403Forbidden, new List<string>());

                    case { RequiresTwoFactor: true }:
                        return ApiResponse<LoginResponseDto>.Failed("Two-factor authentication is required.", StatusCodes.Status401Unauthorized, new List<string>());

                    case { IsNotAllowed: true }:
                        return ApiResponse<LoginResponseDto>.Failed("Login failed. Email confirmation is required.", StatusCodes.Status401Unauthorized, new List<string>());

                    default:
                        return ApiResponse<LoginResponseDto>.Failed("Login failed. Invalid email or password.", StatusCodes.Status401Unauthorized, new List<string>());
                }
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.Failed("Some error occurred while login in." + ex.Message, StatusCodes.Status500InternalServerError, new List<string>() { ex.Message });
            }
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

                 
                stylist.PasswordResetToken = token;
                stylist.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

                await _userManager.UpdateAsync(stylist);

                //var resetPasswordUrl = "http://localhost:3000/confirmpassword?email=" + email + "&token=" + token;


                //var mailRequest = new MailRequest
                //{
                //    ToEmail = email,
                //    Subject = "Your Savi Password Reset Instructions",
                //    Body = $"Please reset your password by clicking <a href='{resetPasswordUrl}'>here</a>."
                //};
                //await _emailServices.SendMailAsync(mailRequest);

                return new ApiResponse<string>(true, "Password reset token retieved successfully.", 200, token, new List<string>());
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


        public async Task<bool> UpdateStylistPortfolioAsync(string userId, List<string> hairStyleIds)
        {
            var user = await _unitOfWork.UserRepository.Query(u => u.Id == userId)
                .Include(u => u.StylistPortfolio)
                .ThenInclude(p => p.HairStyles)
                .FirstOrDefaultAsync();

            if (user == null) return false;

            // Ensure a portfolio exists
            if (user.StylistPortfolio == null)
            {
                user.StylistPortfolio = new StylistPortfolio
                {
                    UserID = userId,
                    HairStyles = new List<HairStyle>()
                };
            }

            // Load selected hairstyles
            var selectedHairStyles = await _unitOfWork.HairStyleRepository
                .Query(h => hairStyleIds.Contains(h.Id))
                .ToListAsync();

            // Update stylist portfolio
            user.StylistPortfolio.HairStyles = selectedHairStyles;

            await _unitOfWork.CompleteAsync();
            return true;
        }


        public async Task<ApiResponse<List<PortfolioResponseDto>>> GetStylistsByHairStyle(string hairStyleId)
        {
            var stylists = await _unitOfWork.UserRepository.GetStylistsByHairStyleAsync(hairStyleId);

            if (stylists == null || !stylists.Any())
            {
                return new ApiResponse<List<PortfolioResponseDto>>(
                    false,
                    "No stylists found for this hairstyle",
                    404,
                    null,
                    new List<string> { "No matching stylists" }
                );
            }

             
            var mappedStylists = stylists.Select(s => new PortfolioResponseDto
            {
                StylistId = s.Id,
                CompanyName = s.CompanyName ?? s.FirstName ?? "Unknown Stylist",
                PhotoUrl = s.ImageUrl ?? "",
                Ratings = s.Rating ?? new List<Ratings>(),

                Town = s.Town ?? ""
            }).ToList();

            return new ApiResponse<List<PortfolioResponseDto>>(
                true,
                "Stylists retrieved successfully",
                200,
                mappedStylists,
                null
            );
        }



        public async Task<List<PortfolioHairStyleDto>?> GetStylistPortfolioAsync(string stylistId)
        {
            // Fetch stylist including portfolio hair styles & photos
            var stylist = await _unitOfWork.UserRepository
                .Query(s => s.Id == stylistId)
                .Include(u => u.StylistPortfolio)
                    .ThenInclude(sp => sp.HairStyles)
                        .ThenInclude(h => h.Photos)
                .FirstOrDefaultAsync();

            if (stylist == null || stylist.StylistPortfolio == null)
                return null;

            // Portfolio contains many hairstyles — use StylistPortfolio.HairStyles
            var result = stylist.StylistPortfolio.HairStyles
                .Where(h => h != null)
                .Select(h => new PortfolioHairStyleDto
                {
                    HairStyleId = h.Id,
                    StyleName = h.StyleName,
                    Origin = h.Origin,
                    Photos = h.Photos
                        .Select(photo => new PhotoDto { Url = photo.Url })
                        .ToList()
                })
                .ToList();

            return result;
        }


    }
}
