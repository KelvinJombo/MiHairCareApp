using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces.Services
{
    public interface IReferralService
    {
        Task<ApiResponse<ReferralResponseDto>> CreateReferralAsync(CreateReferralDto referralDto);
        Task<ApiResponse<IEnumerable<ReferralResponseDto>>> GetAllReferralsAsync();
        Task<ApiResponse<ReferralResponseDto>> GetReferralByIdAsync(string referralDto);      
        Task<ApiResponse<bool>> DeleteReferralAsync(string referralDto);
    }
}
