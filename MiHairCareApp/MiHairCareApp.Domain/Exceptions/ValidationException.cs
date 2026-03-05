namespace MiHairCareApp.Domain.Exceptions
{
    public class ValidationException : DomainException
    {
        public IEnumerable<string>? Errors { get; }

        public ValidationException(string message, IEnumerable<string>? errors = null)
            : base(message, 400)
        {
            Errors = errors;
        }
    }
}
