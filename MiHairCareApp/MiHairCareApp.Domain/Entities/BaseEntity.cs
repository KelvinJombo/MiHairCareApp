using System.ComponentModel.DataAnnotations;

namespace MiHairCareApp.Domain.Entities
{
    public class BaseEntity
    {
        [Required]
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
    }
}
