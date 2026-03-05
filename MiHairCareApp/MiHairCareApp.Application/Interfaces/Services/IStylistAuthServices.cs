using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces
{
    public interface IStylistAuthServices
    { 
        Task<ApiResponse<StylistsRegResponseDto>> RegisterWithGoogleAsync(string idToken, string phoneNumber);
        Task<ApiResponse<StylistsRegResponseDto>> RegisterAsync(CreateStylistsDto createStylistsDto);        
        public Task<ApiResponse<LoginResponseDto>> VerifyAndAuthenticateUserAsync(string idToken);
        Task<ApiResponse<LoginResponseDto>> LoginAsync(StylistsLoginDto loginDTO);
        Task<ApiResponse<string>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<ApiResponse<string>> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
        Task<ApiResponse<string>> ForgotPasswordAsync(string email);
        //Task<ApiResponse<string>> SendConfirmationEmail(string email, string link);
        Task<ApiResponse<string>> ConfirmEmail(string userid, string token);
        ApiResponse<string> ExtractUserIdFromToken(string authToken);
        Task<bool> UpdateStylistPortfolioAsync(string userId, List<string> hairStyleIds);
        Task<ApiResponse<List<PortfolioResponseDto>>> GetStylistsByHairStyle(string hairStyleId);
        Task<ApiResponse<List<RegisterResponseDto>>> GetUsersWithNullCompanyNameAsync();
        Task<List<PortfolioHairStyleDto>?> GetStylistPortfolioAsync(string stylistId);
    }
}
