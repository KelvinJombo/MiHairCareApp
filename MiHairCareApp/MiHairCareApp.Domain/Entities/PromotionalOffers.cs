using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class PromotionalOffers : BaseEntity
    {
          
        public string AppUserID { get; set; }
        public AppUser User { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
