using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class HaircareProduct : BaseEntity
    {
         
        public string ProductName { get; set; }  
        public string Brand { get; set; }  
        public string Description { get; set; }  
        public decimal Price { get; set; }  
        public string ImageUrl { get; set; } 
        public int StockQuantity { get; set; }     
        //public ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();


    }
}
