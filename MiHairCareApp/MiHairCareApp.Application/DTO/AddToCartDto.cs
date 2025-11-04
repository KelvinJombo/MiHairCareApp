namespace MiHairCareApp.Application.DTO
{
    public class AddToCartDto
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CartViewDto
    {
        public IEnumerable<CartItemViewDto> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CartItemViewDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
