namespace MiHairCareApp.Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace MiHairCareApp.Domain.Entities
    {
        public class HairCut
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            [Required]
            [MaxLength(150)]
            public string StyleName { get; set; } = string.Empty;

            [MaxLength(250)]
            public string Description { get; set; } = string.Empty;

            [Required]
            [Column(TypeName = "decimal(18,2)")]
            public decimal PriceTag { get; set; }

            [MaxLength(250)]
            public string ImageUrl { get; set; } = string.Empty;

            [Required]
            public bool IsActive { get; set; } = true;

            [Required]
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? UpdatedAt { get; set; }
        }
    }

}
