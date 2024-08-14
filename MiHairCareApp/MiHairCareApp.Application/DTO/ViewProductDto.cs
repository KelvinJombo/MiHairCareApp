using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class ViewProductDto
    {         
        public string ProductName { get; set; }
        public string Brand { get; set; }        
        public decimal Price { get; set; }         
        public string ImageUrl { get; set; }
        public int StockQuantity { get; set; }
        public string CreatedAt { get; set; }
        
    }
}
