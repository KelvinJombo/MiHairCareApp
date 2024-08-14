using MiHairCareApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class CreateHairStyleDto
    {
        public string StyleName { get; set; }
        public string Description { get; set; }         
        public double PriceTag { get; set; }         
        public HairStyleOrigin Origin { get; set; }
    }
}
