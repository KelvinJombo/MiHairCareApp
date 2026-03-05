using System.ComponentModel.DataAnnotations.Schema;

namespace MiHairCareApp.Domain.Entities
{
    public class StylistPortfolio : BaseEntity
    {      
        [ForeignKey("AppUser")]
        public string UserID { get; set; }
        public AppUser User { get; set; }
        public ICollection<HairStyle> HairStyles { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
