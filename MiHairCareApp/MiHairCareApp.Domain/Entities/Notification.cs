using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Notification : BaseEntity
    {
         
        public string UserID { get; set; }         
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
