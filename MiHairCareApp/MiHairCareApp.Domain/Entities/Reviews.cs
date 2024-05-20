using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Reviews
    {
        [Key]
        public int ReviewID { get; set; }
        public string UserID { get; set; }
        [ForeignKey("StylistID")]
        public string StylistID { get; set; }
        public string ReviewText { get; set; }
        public DateTime DateTime { get; set; }
    }
}
