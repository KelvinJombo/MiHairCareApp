using MiHairCareApp.Domain.Entities;

namespace MiHairCareApp.Application.DTO
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }

    public class CartItemDto
    {
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
