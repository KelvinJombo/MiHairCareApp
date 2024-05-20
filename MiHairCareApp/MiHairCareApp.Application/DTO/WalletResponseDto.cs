using MiHairCareApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public  class WalletResponseDto
    {
        public string WalletNumber { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
    }
}
