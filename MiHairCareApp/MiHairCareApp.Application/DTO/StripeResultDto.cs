using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class StripeResultDto
    {
        public bool PaymentSucceeded { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string CustomerId { get; set; }
        public string PaymentReference { get; set; }
    }
}
