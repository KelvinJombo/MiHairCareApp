namespace MiHairCareApp.Application.DTO
{
    public class BookingRequestDto
    {
        public long Amount { get; set; }   // Stripe requires amount in the smallest currency unit (e.g., cents, kobo)
        public string Currency { get; set; } = "Dollar";  
        public string? Description { get; set; }  
    }

}
