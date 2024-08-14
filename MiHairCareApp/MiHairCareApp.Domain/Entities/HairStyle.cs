using MiHairCareApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class HairStyle : BaseEntity
    {
         
        public string StyleName { get; set; }
        public string Description { get; set; }
        public Photo? Photo { get; set; }
        public string? PhotoId { get; set; }
        public string? VideoLinks { get; set; }
        public double PriceTag { get; set; }         
        public HairStyleOrigin Origin { get; set; }
        public bool PromotionalOffer { get; set; }
        public ICollection<Booking> Bookings { get; set; }

    }
}
