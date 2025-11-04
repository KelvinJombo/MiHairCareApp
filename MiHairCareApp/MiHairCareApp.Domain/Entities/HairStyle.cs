using MiHairCareApp.Domain.Enums;
using System.Text.Json.Serialization;

namespace MiHairCareApp.Domain.Entities
{
    public class HairStyle : BaseEntity
    {
         
        public string StyleName { get; set; }
        public string Description { get; set; }
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public string? PhotoId { get; set; }
        public string? VideoLinks { get; set; }
        public double PriceTag { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public HairStyleOrigin Origin { get; set; }
        public bool PromotionalOffer { get; set; }
        public ICollection<AppUser> Stylists { get; set; }
        public ICollection<Booking> Bookings { get; set; }

    }
}
