using Newtonsoft.Json;

namespace MiHairCareApp.Application.DTO
{
    public class CreateBookingDto 
    {
        public string AppUserId { get; set; }  
        public string HairStyleId { get; set; }  
        public DateTime AppointmentDate { get; set; }
        public bool PaymentCompleted { get; set; }
        public bool Referred { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string? ReferrerUserId { get; set; }  
    }

}
