namespace MiHairCareApp.Domain.Entities
{
    public class SalesRecord
    {
        public int Id { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "GBP";
        public string? Email { get; set; }
        public string? Description { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } = "Pending";
    }

}
