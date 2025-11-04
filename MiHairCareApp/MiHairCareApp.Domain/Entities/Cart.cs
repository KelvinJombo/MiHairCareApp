namespace MiHairCareApp.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public string UserId { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }

    public class CartItem : BaseEntity
    {
        public string ProductId { get; set; }
        public HaircareProduct Product { get; set; }   
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
 
}
