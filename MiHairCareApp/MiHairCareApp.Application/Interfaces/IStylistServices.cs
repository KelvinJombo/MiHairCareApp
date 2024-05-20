using MiHairCareApp.Application.DTO;
using MiHairCareApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.Interfaces
{
    public interface IStylistServices
    {
        Task<ApiResponse<StylistsRegResponseDto>> RegisterAsync(CreateStylistsDto createStylistsDto);
        Task<ApiResponse<StylistsLoginResponseDto>> LoginAsync(StylistsLoginDto loginDTO);
    }
}
