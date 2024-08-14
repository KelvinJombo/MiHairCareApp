using MiHairCareApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class CreateBookingDto
    {        
        public AppUser User { get; set; }         
        public HairStyle HairStyle { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool PaymentCompleted { get; set; }
        public bool Referred { get; set; }
        public string? ReferralID { get; set; }         
    }
}
