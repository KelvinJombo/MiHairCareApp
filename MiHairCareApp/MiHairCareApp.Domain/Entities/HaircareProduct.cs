using MiHairCareApp.Domain.Enums;

namespace MiHairCareApp.Domain.Entities
{
    public class HaircareProduct : BaseEntity
    {
        public string ProductName { get; set; }
        public string Brand { get; set; }
        public ProductCategory Category { get; set; }    
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
    }

}
