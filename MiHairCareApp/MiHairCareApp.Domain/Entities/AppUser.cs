using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace MiHairCareApp.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public string Street { get; set; } = string.Empty;     
        public string Town { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;      
        public string State { get; set; } = string.Empty;     
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;        
        public string Address { get; set; } = string.Empty;
        public int LoyaltyPoints { get; set; }
        public DateTime LastLogin { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool HomeService { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }        
        public string? CompanyName { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public int PromotionalOfferID { get; set; }
        public ICollection<PromotionalOffers> PromotionalOffers { get; set; } = new List<PromotionalOffers>();
        public ICollection<Referral> ReferralsReceived { get; set; } = new List<Referral>();
        public ICollection<Referral> ReferralsMade { get; set; } = new List<Referral>();
        public int BookingID { get; set; }
        public TimeSpan StartHour { get; set; }
        public TimeSpan EndHour { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Ratings> Rating { get; set; } = new List<Ratings>();
        public string? StylePortfolioID { get; set; }
        public StylistPortfolio? StylistPortfolio { get; set; }
        public ICollection<Review> Review { get; set; } = new List<Review>();
    }




}