using MiHairCareApp.Domain.Enums;

namespace MiHairCareApp.Application.DTO
{
    public class AllHairStylesResponseDto
    {
        public string StyleName { get; set; } = string.Empty;        
        public HairStyleOrigin Origin { get; set; }
        public string HairStyleId { get; set; } = string.Empty;       
        public ICollection<PhotoDto> Photos { get; set; }
    }
}
