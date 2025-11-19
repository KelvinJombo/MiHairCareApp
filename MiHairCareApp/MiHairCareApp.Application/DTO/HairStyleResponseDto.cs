using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.DTO
{
    public class HairStyleResponseDto
    {
        public string StyleName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HairStyleId { get; set; } = string.Empty;
        public double PriceTag { get; set; }
        public bool PromotionalOffer { get; set; }         
        public ICollection<PhotoDto> Photos { get; set; }

    }
}
