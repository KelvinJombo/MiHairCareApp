using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Stylist : IdentityUser
    {
        public string StylistName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public string Address { get; set; }
        public string Town { get; set; }
        public string Street { get; set; }
        public double DistanceInKM { get; set; }
        public bool HomeService { get; set; }
        public DateTime LastLogin { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("PromotionalOffer")]
        public int PromotionalOfferID { get; set; }
        public ICollection<PromotionalOffers> PromotionalOffers { get; set; }
        [ForeignKey("BookingID")]
        public int BookingID { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        [ForeignKey("StylistPortfolio")]
        public string StylePortfolioID { get; set; }
        public StylistPortfolio StylistPortfolio { get; set; }
        public string BookingLink { get; set; }
        public string PhoneNumber { get; set; }
    }
}
