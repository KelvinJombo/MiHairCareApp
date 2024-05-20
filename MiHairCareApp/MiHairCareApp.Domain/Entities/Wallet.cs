using MiHairCareApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public string WalletNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
        public string PaystackCustomerCode { get; set; } = string.Empty;
        public string TransactionPin { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public string UserId { get; set; } = string.Empty;
        public ICollection<WalletFunding> WalletFundings { get; set; } = new List<WalletFunding>();
        
        public ICollection<UserTransaction> UserTransactions { get; set; }
    }
}
