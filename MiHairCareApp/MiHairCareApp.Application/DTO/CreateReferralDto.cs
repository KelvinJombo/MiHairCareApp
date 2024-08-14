using MiHairCareApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class CreateReferralDto
    { 
        public string ReferrerUserId { get; set; }        
        public string ReferredUserId { get; set; }        
        public string StylistID { get; set; }
         
    }
}
