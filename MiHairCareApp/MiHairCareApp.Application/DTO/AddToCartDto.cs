namespace MiHairCareApp.Application.DTO
{
    public class AddToCartDto
    {         
        public string Id { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
    }


    public class CartViewDto
    {
        public IEnumerable<CartItemViewDto> Items { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class CartItemViewDto
    {
        public string Id { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
