namespace MiHairCareApp.Application.DTO
{
    public class CreatePaymentRequestDto
    {
        public long Amount { get; set; }
        public string Currency { get; set; } = "GBP";
        public string? Description { get; set; }
        public string? CustomerEmail { get; set; }
        public CustomerDto Customer { get; set; }
    }

    


}
