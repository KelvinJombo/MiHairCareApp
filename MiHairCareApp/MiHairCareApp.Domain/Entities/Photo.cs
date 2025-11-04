namespace MiHairCareApp.Domain.Entities
{
    public class Photo : BaseEntity
    {
        
        public bool IsMain { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public AppUser? AppUser { get; set; }
        public string? UserId { get; set; }     
        public HairStyle HairStyle { get; set; }     
        public HaircareProduct HaircareProduct { get; set; }     
        public string? HaircareProductId { get; set; }     
        public string? HairStyleId { get; set; }
    }
} 
