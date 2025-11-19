using Microsoft.AspNetCore.Http;

namespace MiHairCareApp.Application.DTO
{
    public class CreateProductDto
    {
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile? Image { get; set; }
        public int StockQuantity { get; set; }
        public bool IsMainPhoto { get; set; } = true;
    }

}
