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
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();         
        public int StockQuantity { get; set; }
    }

}
