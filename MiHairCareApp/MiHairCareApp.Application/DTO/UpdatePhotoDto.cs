using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class UpdatePhotoDto
    {
        public IFormFile Image { get; set; }
        public string UserId { get; set; }
    }
}
