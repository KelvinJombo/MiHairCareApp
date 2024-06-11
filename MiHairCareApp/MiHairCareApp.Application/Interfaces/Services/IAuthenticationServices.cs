using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IAuthenticationServices
    {
        //Task<ApiResponse<string>> ValidateTokenAsync(string token);
        ApiResponse<string> ExtractUserIdFromToken(string authToken);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(AppUserLoginDto loginDTO);
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(UserCreateDto createDto);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task<ApiResponse<string>> ForgotPasswordAsync(string email);
        //Task<ApiResponse<string>> SendConfirmationEmail(string email, string link);
        Task<ApiResponse<string>> ConfirmEmail(string userid, string token);
        //Task<ApiResponse<string[]>> VerifyAndAuthenticateUserAsync(string idToken); 
    }
}
