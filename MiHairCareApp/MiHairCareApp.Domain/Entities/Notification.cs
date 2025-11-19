namespace MiHairCareApp.Domain.Entities
{
    public class Notification : BaseEntity
    {         
        public string UserID { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }
    }
}
