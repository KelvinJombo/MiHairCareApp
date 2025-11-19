using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace MiHairCareApp.Domain.Entities
{

    public class AppUser : IdentityUser
    {
        // ---------------------------
        // BASIC USER INFORMATION
        // ---------------------------

        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        // Email & Phone already exist in IdentityUser (Email, PhoneNumber)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }


        // ---------------------------
        // ADDRESS INFORMATION
        // ---------------------------

        public string Street { get; set; } = string.Empty;    // From AppUser
        public string Town { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;      // From Customer
        public string State { get; set; } = string.Empty;     // From Customer
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        // For IdentityUser, Address doesn't exist, so:
        public string Address { get; set; } = string.Empty;


        // ---------------------------
        // ACCOUNT / PROFILE DETAILS
        // ---------------------------

        public int LoyaltyPoints { get; set; }
        public DateTime LastLogin { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool HomeService { get; set; }

        // Geo Positioning
        public int Longitude { get; set; }
        public int Latitude { get; set; }

        // Company or Business Name (Optional)
        public string? CompanyName { get; set; }

        // Password Reset
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }


        // ---------------------------
        // E-COMMERCE / ORDER DETAILS
        // ---------------------------

        public ICollection<Order> Orders { get; set; } = new List<Order>();


        // ---------------------------
        // PROMOTIONAL OFFERS
        // ---------------------------

        public int PromotionalOfferID { get; set; }
        public ICollection<PromotionalOffers> PromotionalOffers { get; set; } = new List<PromotionalOffers>();


        // ---------------------------
        // REFERRALS
        // ---------------------------

        public ICollection<Referral> ReferralsReceived { get; set; } = new List<Referral>();
        public ICollection<Referral> ReferralsMade { get; set; } = new List<Referral>();


        // ---------------------------
        // BOOKINGS & STYLIST FEATURES
        // ---------------------------

        public int BookingID { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // Styles the user can offer
        public ICollection<HairStyle> AvailableStyles { get; set; } = new List<HairStyle>();


        // Stylist Portfolio
        public string? StylePortfolioID { get; set; }
        public StylistPortfolio? StylistPortfolio { get; set; }


        // ---------------------------
        // REVIEWS
        // ---------------------------

        public ICollection<Review> Review { get; set; } = new List<Review>();
    }




}