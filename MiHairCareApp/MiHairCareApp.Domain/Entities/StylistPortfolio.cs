using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class StylistPortfolio : BaseEntity
    {
         
        public string StylePicturesLink { get; set; }
        public string StyleVideoLink { get; set; }
        [ForeignKey("StylistID")]
        public string StylistID { get; set; }
        public Stylist Stylist { get; set; }
    }
}
