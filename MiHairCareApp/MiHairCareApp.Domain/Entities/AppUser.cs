using Microsoft.AspNetCore.Identity;

namespace MiHairCareApp.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PasswordResetToken { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }
        public DateTime LastLogin { get; set; }

        public ICollection<Referral> ReferralsMade { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}