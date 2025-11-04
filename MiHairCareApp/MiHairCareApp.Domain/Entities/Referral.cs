using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Referral : BaseEntity 
    {        
        public string ReferrerUserId { get; set; }
        public AppUser ReferrerUser { get; set; }
        public string ReferredUserId { get; set; }
        public AppUser ReferredUser { get; set; }                 
        public string StylistID { get; set; }
        public AppUser Stylist { get; set; }         
        public ICollection<Booking> Bookings { get; set; } 
    }

}
