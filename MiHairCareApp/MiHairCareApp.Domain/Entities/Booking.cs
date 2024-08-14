using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Booking : BaseEntity
    {         

         
        public string AppUserId { get; set; }
        public AppUser User { get; set; }         
        public string HairStyleID { get; set; }
        public HairStyle HairStyle { get; set; }        
        public DateTime AppointmentDate { get; set; }         
        public bool PaymentCompleted { get; set; }         
        public bool Referred { get; set; }         
        public Guid? ReferralID { get; set; }
        public Referral Referral { get; set; }
    }

}
