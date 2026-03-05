using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IHairStyleServices
    {
        Task<ApiResponse<HairStyleResponseDto>> AddHairStyleAsync(CreateHairStyleDto hairDto);
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAfricanHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAmericanHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllAsianHairStyles();
        Task<ApiResponse<List<HairStyleResponseDto>>> GetAllEuropeanHairStyles();
        Task<ApiResponse<HairStyleResponseDto>> GetHairStyleById(string hairStyleId);
        Task<ApiResponse<HairStyleResponseDto>> GetHairStyleByTitle(string hairStyleTitle);
        //Task<ApiResponse<List<AllHairStylesResponseDto>>> GetStylistPortFolioAsync();
        Task<ApiResponse<bool>> DeleteAHairStyle(string hairStyleId);
        Task<ApiResponse<PhotoDto>> AddHairStylePhoto(UpdateHairStylePhotoDto updatePhotoDto);
        Task<ApiResponse<string>> GetHairStylePhotoAsync(string photoId);
        Task<bool> DeleteHairStylePhotoAsync(string photoId);
        Task<ApiResponse<HairStyleResponseDto>> UpdateHairStyleAsync(UpdateHairStylePhotoDto updatePhotoDto);
        Task<ApiResponse<List<PortfolioHairStyleDto>>> GetStylistPortfolioAsync(string userId);
    }
}
