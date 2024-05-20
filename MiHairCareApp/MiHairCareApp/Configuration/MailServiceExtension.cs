using MiHairCareApp.Domain.Entities.Helper;

namespace MiHairCareApp.Configuration
{
    public static class MailServiceExtension
    {
        public static void AddMailService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        }
    }
}
