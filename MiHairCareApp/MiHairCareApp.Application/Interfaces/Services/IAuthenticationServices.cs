using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IAuthenticationServices
    {
        Task<ApiResponse<string[]>> RegisterWithGoogleAsync(string idToken, string phoneNumber);
        //Task<ApiResponse<string>> ValidateTokenAsync(string token);
        ApiResponse<string> ExtractUserIdFromToken(string authToken);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(AppUserLoginDto loginDTO);
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(UserCreateDto createDto);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task<ApiResponse<string>> ForgotPasswordAsync(string email);
        //Task<ApiResponse<string>> SendConfirmationEmail(string email, string link);
        Task<ApiResponse<string>> ConfirmEmail(string userid, string token);
        public Task<ApiResponse<LoginResponseDto>> VerifyAndAuthenticateUserAsync(string idToken);


    }
}
