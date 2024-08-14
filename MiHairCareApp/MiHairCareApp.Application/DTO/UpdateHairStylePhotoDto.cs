using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class UpdateHairStylePhotoDto
    {
        public IFormFile Image { get; set; }
        public string HairStyleId { get; set; }
        public bool IsMain { get; set; }
    }
}
