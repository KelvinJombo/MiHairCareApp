using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IHairStyleServices
    {
        Task<ApiResponse<HairStyleResponseDto>> AddHairStyleAsync(CreateHairStyleDto hairDto);
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAfricanHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAmericanHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAsianHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllEuropianHairStyles();
        Task<ApiResponse<HairStyleResponseDto>> GetHairStyleById(string hairStyleId);
        Task<ApiResponse<bool>> DeleteAHairStyle(string hairStyleId);
        Task<ApiResponse<PhotoDto>> AddHairStylePhoto(UpdateHairStylePhotoDto updatePhotoDto);
        Task<ApiResponse<string>> GetHairStylePhotoAsync(string photoId);
        Task<bool> DeleteHairStylePhotoAsync(string photoId);

    }
}
