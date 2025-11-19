using MiHairCareApp.Domain.Enums;
using System.Text.Json.Serialization;

namespace MiHairCareApp.Domain.Entities
{
    public class UserTransaction : BaseEntity
    {
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public long Amount { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Currency Currency { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public string PaymentReference { get; set; } = string.Empty;
        public bool PaymentSucceeded { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;        
        public string Status { get; set; } = "pending";
    }
}


