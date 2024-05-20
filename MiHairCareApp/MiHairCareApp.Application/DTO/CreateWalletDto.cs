using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class CreateWalletDto
    {
        public string PhoneNumber { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public string UserId { get; set; } = string.Empty;
    }
}
