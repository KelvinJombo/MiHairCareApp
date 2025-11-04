namespace MiHairCareApp.Application.DTO
{
    public class RegisterResponseDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastLogin { get; set; }
        public string IsDeleted { get; set; }
        public string ImageUrl { get; set; }
        public int Longitude { get; set; }
        public int Latitude { get; set; }
    }
}
