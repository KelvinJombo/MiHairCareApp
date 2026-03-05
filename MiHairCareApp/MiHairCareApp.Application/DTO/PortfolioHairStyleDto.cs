using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Enums;

namespace MiHairCareApp.Application.DTO
{
    public class PortfolioHairStyleDto
    {
        public string HairStyleId { get; set; }
        public string StyleName { get; set; }
        public HairStyleOrigin Origin { get; set; }
        public List<PhotoDto> Photos { get; set; }
    }
}
