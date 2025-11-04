using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class WalletFunding : BaseEntity
    {
        public double FundAmount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Narration { get; set; } = string.Empty;
        //public TransactionType TransactionType { get; set; }
        [ForeignKey("ActionId")]
        public string ActionId { get; set; }
        public decimal CumulativeAmount { get; set; }
        public string WalletNumber { get; set; }

        [ForeignKey("WalletId")]
        public string WalletId { get; set; } = string.Empty;
    }
}
