using CloudinaryDotNet;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MiHairCareApp.Application.DTO;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Entities.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using static Google.Apis.Requests.BatchRequest;





namespace MiHairCareApp.Application.ServicesImplementation
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthenticationServices> _logger;
        //private readonly IWalletServices _walletService;
        private readonly IEmailServices _emailServices;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public AuthenticationServices(UserManager<AppUser> userManager,ILogger<AuthenticationServices> logger, 
                                      IEmailServices emailServices,IUnitOfWork unitOfWork,IConfiguration config,SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _logger = logger;
            //_walletService = walletService;
            _emailServices = emailServices;
            _unitOfWork = unitOfWork;
            _config = config;
            _signInManager = signInManager;

            _secretKey = _config["JwtSettings:Secret"];
            _issuer = _config["JwtSettings:Issuer"];
            _audience = _config["JwtSettings:Audience"];
        }


        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(UserCreateDto createDto)
        {
            var user = await _userManager.FindByEmailAsync(createDto.Email);
            if (user != null)
            {
                return ApiResponse<RegisterResponseDto>.Failed("User with this email already exists.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var userr = await _unitOfWork.UserRepository.FindAsync(x => x.PhoneNumber == createDto.PhoneNumber);
            if (userr.Count > 0)
            {
                return ApiResponse<RegisterResponseDto>.Failed("User with this phone number already exists.", StatusCodes.Status400BadRequest, new List<string>());
            }

            var appUser = new AppUser()
            {
                FirstName = createDto.FirstName,
                UserName = createDto.UserName,
                Email = createDto.Email,
                PhoneNumber = createDto.PhoneNumber,
                PasswordResetToken = ""
            };

            try
            {
                var token = "";

                var result = await _userManager.CreateAsync(appUser, createDto.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(appUser, "User");
                   // token = 


                    //var createWalletDto = new CreateWalletDto
                    //{
                    //    PhoneNumber = appUser.PhoneNumber,
                    //    UserId = appUser.Id
                    //};

                     token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    if (!String.IsNullOrEmpty(token))
                    {
                        var response = new RegisterResponseDto
                        {
                            Id = appUser.Id,
                            Email = appUser.Email,
                            PhoneNumber = appUser.PhoneNumber,
                            FirstName = appUser.FirstName,
                            UserName = appUser.UserName,
                            Token = token
                        };



                        return ApiResponse<RegisterResponseDto>.Success(response, "User registered successfully. Please click on the link sent to your email to confirm your account", StatusCodes.Status201Created);
                    }
                    else
                    {
                        _unitOfWork.UserRepository.DeleteAsync(appUser);
                        return ApiResponse<RegisterResponseDto>.Failed("User creation failed", StatusCodes.Status400BadRequest, new List<string>());
                    }
                }
                else
                {
                    return ApiResponse<RegisterResponseDto>.Failed("Error occurred: Failed to create User", StatusCodes.Status400BadRequest, new List<string>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding this User " + ex.InnerException);
                return ApiResponse<RegisterResponseDto>.Failed("Error creating user.", StatusCodes.Status500InternalServerError, new List<string>() { ex.InnerException.ToString() });
            }
        }




        public async Task<ApiResponse<string[]>> VerifyAndAuthenticateUserAsync(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
                var userEmail = payload.Email;
                var existingUser = await _userManager.FindByEmailAsync(userEmail);
                if (existingUser == null)
                {
                    return ApiResponse<string[]>.Failed("Email not registered", StatusCodes.Status404NotFound, new List<string>());
                }
                //var wallet = await _walletService.GetWalletByUserId(existingUser.Id);
                //string wallletNumber = wallet.Data.WalletNumber;
                string userId = existingUser.Id;
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

                string[] response = new string[] { userEmail, user.FirstName + " " + user.UserName, userId };


                if (existingUser != null)
                {
                    await _signInManager.SignInAsync(existingUser, isPersistent: false);
                    return ApiResponse<string[]>.Success(response, "User Logged in successfully", StatusCodes.Status200OK);
                }
                else
                {
                    return ApiResponse<string[]>.Failed("User not found", StatusCodes.Status404NotFound, new List<string>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                return ApiResponse<string[]>.Failed("Error occurred while authenticating user", StatusCodes.Status500InternalServerError, new List<string> { ex.Message });
            }
        }




        public async Task<ApiResponse<string>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config.GetSection("JwtSettings:Secret").Value);

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


         



        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(AppUserLoginDto loginDTO)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    return ApiResponse<LoginResponseDto>.Failed("User not found.", StatusCodes.Status404NotFound, new List<string>());
                }
                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: false);

                switch (result)
                {
                    case { Succeeded: true }:
                        var role = (await _userManager.GetRolesAsync(user)).First();
                        user.LastLogin = DateTime.Now;
                        _unitOfWork.UserRepository.Update(user);
                        await _unitOfWork.SaveChangesAsync();
                        var response = new LoginResponseDto
                        {
                            JWToken = GenerateJwtToken(user.Id, user.Email, role)
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
                return ApiResponse<LoginResponseDto>.Failed("Some error occurred while loging in. " + ex.Message, StatusCodes.Status500InternalServerError, new List<string>() { ex.Message });
            }
        }


        //private string GenerateJwtToken(AppUser user, string role)
        //{
        //    var jwtSettings = _config.GetSection("JwtSettings:Secret").Value;
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName+" "+user.UserName),
        //        new Claim(ClaimTypes.Role, role)
        //    };
        //    var token = new JwtSecurityToken(
        //        issuer: _config.GetValue<string>("JwtSettings:ValidIssuer"),
        //        audience: _config.GetValue<string>("JwtSettings:ValidAudience"),
        //    //issuer: null,
        //    //audience: null,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(int.Parse(_config.GetSection("JwtSettings:AccessTokenExpiration").Value)),
        //        signingCredentials: credentials
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}





        public string GenerateJwtToken(string userId, string email, string role)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("Email cannot be null or empty", nameof(email));
            if (string.IsNullOrEmpty(role)) throw new ArgumentException("Role cannot be null or empty", nameof(role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(50),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }


         


        public async Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            if (user == null)
            {
                return new ApiResponse<string>(false, "Invalid model state.", 400, null, new List<string>());
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return new ApiResponse<string>(false, "Authorization token is missing.", 401, null, new List<string>());
            }



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
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                return new ApiResponse<string>(false, "Input Correct Strings Formats", 400, null, new List<string>());
            }



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
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    return new ApiResponse<string>(false, "Email does not exist", StatusCodes.Status404NotFound, null, new List<string>());
                }

                if (!user.EmailConfirmed)
                {
                    return new ApiResponse<string>(false, "User email not confirmed.", StatusCodes.Status404NotFound, null, new List<string>());
                }

                string token = await _userManager.GeneratePasswordResetTokenAsync(user);

                //  token = HttpUtility.UrlEncode(token);

                user.PasswordResetToken = token;
                user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

                await _userManager.UpdateAsync(user);

                //var resetPasswordUrl = "https://localhost:7261/confirmpassword?email=" + email + "&token=" + token;
                ////var link = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);


                //var mailRequest = new MailRequest
                //{
                //    ToEmail = email,
                //    Subject = "Your Savi Password Reset Instructions",
                //    Body = $"Please reset your password by clicking <a href='{resetPasswordUrl}'>here</a>."
                //};
                //await _emailServices.SendMailAsync(mailRequest);

                return new ApiResponse<string>(true, "Password reset token returned successfully.", 200, token, new List<string>());
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
                return ApiResponse<string>.Failed("User id or token cannot be null", StatusCodes.Status400BadRequest, new List<string>() { });
            }

            var user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return ApiResponse<string>.Failed("Invalid User", StatusCodes.Status404NotFound, new List<string>() { });
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
            

            if (string.IsNullOrWhiteSpace(authToken))
            {
                return new ApiResponse<string>(false, "Authorization token is missing.", 401, null, new List<string>());
            }


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
