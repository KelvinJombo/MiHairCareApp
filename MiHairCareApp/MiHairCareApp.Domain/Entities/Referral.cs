using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Referral
    {
        [Key]
        public Guid ReferralID { get; set; }

        // Foreign key for the AppUser who made the referral
        public string ReferrerUserId { get; set; }
        public AppUser ReferrerUser { get; set; }

        // Foreign key for the Stylist associated with the referral
        public string UserID { get; set; }
        public AppUser User { get; set; }

        // Navigation property for bookings resulting from this referral
        public ICollection<Booking> Bookings { get; set; }
    }

}
