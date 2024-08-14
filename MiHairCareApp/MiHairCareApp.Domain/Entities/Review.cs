using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Review : BaseEntity
    {
        [ForeignKey(nameof(AppUser))]
        public string UserID { get; set; }
        public AppUser User { get; set; }

        [ForeignKey(nameof(Stylist))]
        public string StylistID { get; set; }
        public AppUser Stylist { get; set; }

        public string ReviewText { get; set; }
    }


}
