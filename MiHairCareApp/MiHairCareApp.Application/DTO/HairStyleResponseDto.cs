using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class HairStyleResponseDto
    {
        public string StyleName { get; set; }
        public string Description { get; set; }
        public string ImagesLinks { get; set; }         
        public double PriceTag { get; set; }
        public bool PromotionalOffer { get; set; }
    }
}
