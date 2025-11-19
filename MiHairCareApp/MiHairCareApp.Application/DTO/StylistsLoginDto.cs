using System.ComponentModel.DataAnnotations;

namespace MiHairCareApp.Application.DTO
{
    public class StylistsLoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
