using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class UserTransaction : BaseEntity
    {         
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ReceiverWalletNumber { get; set; } = string.Empty;
        public string SenderWalletNumber { get; set; } = string.Empty;

         
    }
}
