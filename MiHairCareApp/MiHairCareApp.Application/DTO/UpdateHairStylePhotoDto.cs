using Microsoft.AspNetCore.Http;

namespace MiHairCareApp.Application.DTO
{
    public class UpdateHairStylePhotoDto
    {
        public string HairStyleId { get; set; }
        public string StyleName { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }         
        public bool IsMain { get; set; }
    }
}
