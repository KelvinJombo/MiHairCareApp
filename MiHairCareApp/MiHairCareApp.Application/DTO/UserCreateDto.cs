using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Application.DTO
{
    public class UserCreateDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Invalid username")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "Invalid username")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
    }
}
