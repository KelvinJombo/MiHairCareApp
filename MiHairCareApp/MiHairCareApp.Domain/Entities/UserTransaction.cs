using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class UserTransaction : BaseEntity
    {
        public string CustomerId { get; set; }
        public string Description { get; set; } = string.Empty;
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public bool PaymentSucceeded { get; set; }

         
    }
}
