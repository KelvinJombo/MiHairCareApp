using global::MiHairCareApp.Domain.Enums;

namespace MiHairCareApp.Domain.Entities
{
    public class Order : BaseEntity
    {
        // Primary Key
        public int OrderId { get; set; }

        public string PaymentIntentId { get; set; }
        public string UserId { get; set; }

        // Order Details
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? ShippedDate { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal SubTotal { get; set; }
        public Currency Currency { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal TotalAmount => SubTotal + Tax + ShippingFee;

        // Shipping Information
        public string ShippingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Payment Information
        public string PaymentMethod { get; set; }
        public bool IsPaid { get; set; }

        // Navigation Properties
        public AppUser User { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }




}


