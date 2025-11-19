namespace MiHairCareApp.Application.DTO
{
    public class ViewProductDto
    {
        public string Id { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
    }


}
