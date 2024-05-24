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

        // Foreign key for the AppUser who made the booking
        public string AppUserId { get; set; }
        public AppUser User { get; set; }

        // Foreign key for the HairStyle booked
        public string HairStyleID { get; set; }
        public HairStyle HairStyle { get; set; }

        // Appointment details
        public DateTime AppointmentDate { get; set; }
         
        // Indicates if payment for the booking has been completed
        public bool PaymentCompleted { get; set; }

        // Indicates if the booking was made as a result of a referral
        public bool Referred { get; set; }

        // Foreign key for the Referral associated with the booking (if applicable)
        public Guid? ReferralID { get; set; }
        public Referral Referral { get; set; }
    }

}
