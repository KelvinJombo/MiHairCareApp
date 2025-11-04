using MiHairCareApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class UserTransaction : BaseEntity
    {
        public string CustomerId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReceiptEmail { get; set; } = string.Empty;
        public long Amount { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Currency Currency { get; set; }
        public string PaymentReference { get; set; } = string.Empty;
        public bool PaymentSucceeded { get; set; }

         
    }
}
