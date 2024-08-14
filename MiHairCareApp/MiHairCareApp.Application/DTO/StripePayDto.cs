using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class StripePayDto
    {
        public string StripeEmail { get; set; }
        public string StripeToken { get; set; }
        public long Amount { get; set; }  
        public string Description { get; set; }
        public string Currency { get; set; }
    }
}
