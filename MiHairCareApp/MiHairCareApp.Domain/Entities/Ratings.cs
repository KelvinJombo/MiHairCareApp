using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Ratings : BaseEntity
    {
         
        public string UserID { get; set; }
        public string StylistID { get; set; }
        public int Rating { get; set; }
        public DateTime DateTime { get; set; }
    }
}
