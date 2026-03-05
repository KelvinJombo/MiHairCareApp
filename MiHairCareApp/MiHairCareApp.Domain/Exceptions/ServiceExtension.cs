namespace MiHairCareApp.Domain.Exceptions
{
    public class ServiceException : DomainException
    {
        public ServiceException(string message, Exception? inner = null)
            : base(message, 500, inner) { }
    }
}
