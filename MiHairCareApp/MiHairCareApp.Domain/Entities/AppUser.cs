using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiHairCareApp.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PasswordResetToken { get; set; } = string.Empty;
        //public bool IsDeleted { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }
        public DateTime LastLogin { get; set; }             
        public string? CompanyName { get; set; }            
        public string? Town { get; set; }
        public string? Street { get; set; }
        public double DistanceInKM { get; set; }
        public bool HomeService { get; set; }         
        [ForeignKey("PromotionalOffer")]
        public int PromotionalOfferID { get; set; }
        public ICollection<PromotionalOffers>? PromotionalOffers { get; set; }
        [ForeignKey("BookingID")]
        public int BookingID { get; set; }
        public ICollection<Booking>? Bookings { get; set; }         
        public ICollection<Referral>? ReferralsMade { get; set; }
        [ForeignKey("StylistPortfolio")]
        public string? StylePortfolioID { get; set; }
        public StylistPortfolio? StylistPortfolio { get; set; }
        //public string? BookingLink { get; set; }
         
    }
}