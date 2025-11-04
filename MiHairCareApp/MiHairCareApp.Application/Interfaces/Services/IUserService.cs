using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using MiHairCareApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<ApiResponse<List<RegisterResponseDto>>> GetStylistUsers();
        Task<ApiResponse<bool>> DeleteUser(string id);
        //Task<ApiResponse<List<NewUserResponseDto>>> GetNewUsers();
        //Task<ApiResponse<decimal[]>> AdminDashboardUserInfo();
        Task<ApiResponse<GetUserResponseDto>> GetUserById(string userId);
        Task<ApiResponse<PhotoDto>> AddPhoto(UpdatePhotoDto updatePhotoDto);
        Task<ApiResponse<string>> GetPhoto(string photoId);
        Task<bool> DeletePhotoAsync(string photoId);
        //Task<List<AppUser>> GetStylistsByHairStyleAsync(string hairStyleId);
        Task<ApiResponse<List<RegisterResponseDto>>> GetStylistsByHairStyle(string hairStyleId);
        Task<ApiResponse<List<RegisterResponseDto>>> GetUsersWithNullCompanyNameAsync();
        


    }
}
