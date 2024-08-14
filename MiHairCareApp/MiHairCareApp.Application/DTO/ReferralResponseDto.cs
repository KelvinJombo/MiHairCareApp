using MiHairCareApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class ReferralResponseDto
    {
        public Guid ReferralID { get; set; }            
        public string StylistID { get; set; }
        public AppUser ReferredUserId { get; set; }
    }
}
