namespace MiHairCareApp.Domain.Entities
{
    public class OrderItem
    {
        // Primary Key
        public int OrderItemId { get; set; }

        // Foreign Key to Order
        public int OrderId { get; set; }

        // Product Details
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // Navigation Property
        public Order Order { get; set; }
    }
}
