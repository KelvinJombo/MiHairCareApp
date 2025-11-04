using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiHairCareApp.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? PasswordResetToken { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }
        public DateTime LastLogin { get; set; }
        public string? CompanyName { get; set; }
        public string? Town { get; set; }
        public string? Street { get; set; }       
        public bool HomeService { get; set; }

        // Navigation properties
        public int PromotionalOfferID { get; set; }
        public ICollection<PromotionalOffers> PromotionalOffers { get; set; } = new List<PromotionalOffers>();
        public int Longitude { get; set; }
        public int Latitude { get; set; }        
        public int BookingID { get; set; }
        public ICollection<Referral> ReferralsReceived { get; set; } = new List<Referral>();
        public ICollection<Referral> ReferralsMade { get; set; } = new List<Referral>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<HairStyle> AvailableStyles { get; set; } = new List<HairStyle>();
        public string? StylePortfolioID { get; set; }
        public StylistPortfolio? StylistPortfolio { get; set; }
        public ICollection<Review> Review { get; set; } = new List<Review>();
    }



}