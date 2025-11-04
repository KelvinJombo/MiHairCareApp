using Microsoft.AspNetCore.Http;

namespace MiHairCareApp.Application.DTO
{
    public class UpdateProductsDto
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public IFormFile Image { get; set; }         
        public int StockQuantity { get; set; }
         

         

    }
}
