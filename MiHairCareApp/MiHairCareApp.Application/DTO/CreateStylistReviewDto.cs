using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class CreateStylistReviewDto
    {
        public string UserID { get; set; }                
        public string StylistID { get; set; }                
        public string ReviewText { get; set; }
    }
}
