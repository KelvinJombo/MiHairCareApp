using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.DTO
{
    public class HairStyleResponseDto
    {
        public string StyleName { get; set; }
        public string Description { get; set; }
       // public string ImagesLinks { get; set; }         
        public double PriceTag { get; set; }
        public bool PromotionalOffer { get; set; }
        //public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<PhotoDto> Photos { get; set; }

    }
}
