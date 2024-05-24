using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class CreateStylistsDto
    {
        
        public string StylistName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public bool HomeService { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
    }
}
