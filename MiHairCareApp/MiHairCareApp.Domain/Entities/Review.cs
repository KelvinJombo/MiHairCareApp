using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiHairCareApp.Domain.Entities
{
    public class Review : BaseEntity
    {
        [ForeignKey(nameof(AppUser))]
        public string UserID { get; set; }               // Foreign key to AppUser
        public AppUser User { get; set; }                // Navigation property

        public string? StylistID { get; set; }           // Optional foreign key for Stylist
        public string? ProductId { get; set; }           // Optional foreign key for Product

        public string ReviewText { get; set; } = string.Empty; // Ensure default value
    }



}
