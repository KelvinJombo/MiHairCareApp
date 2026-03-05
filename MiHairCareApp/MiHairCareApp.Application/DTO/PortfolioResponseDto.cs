using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.DTO
{
    public class PortfolioResponseDto
    {
        public string StylistId { get; set; }
        public string CompanyName { get; set; }
        public string PhotoUrl { get; set; }
        public ICollection<Ratings> Ratings { get; set; }
        public string Town { get; set; }
    }



}
