using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Action : BaseEntity
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
    }
}
