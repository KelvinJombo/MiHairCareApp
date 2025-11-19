using Microsoft.AspNetCore.Http;
using MiHairCareApp.Domain.Enums;
using System.Text.Json.Serialization;

namespace MiHairCareApp.Application.DTO
{
    public class CreateHairStyleDto
    {
        public string StyleName { get; set; }
        public string Description { get; set; }
        public double PriceTag { get; set; }
        public string Origin { get; set; }
        [JsonIgnore]
        public HairStyleOrigin OriginEnum { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsMainPhoto { get; set; } = true;
    }


}
