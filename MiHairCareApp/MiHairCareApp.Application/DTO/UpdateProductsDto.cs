using Microsoft.AspNetCore.Http;

namespace MiHairCareApp.Application.DTO
{
    public class UpdateProductsDto
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public IFormFile? Image { get; set; }
        public bool IsMainPhoto { get; set; } = true;
        public int StockQuantity { get; set; }         

    }
}
