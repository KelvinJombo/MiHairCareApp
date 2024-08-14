using MiHairCareApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class UpdateBookingDto
    {
        public string  BookingID { get; set; }
        public string HairStyleID { get; set; }         
        public DateTime AppointmentDate { get; set; }
        public bool PaymentCompleted { get; set; }
        public bool Referred { get; set; }
        public Guid? ReferralID { get; set; }        
    }
}
