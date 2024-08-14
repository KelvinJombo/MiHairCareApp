using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using MiHairCareApp.Domain.Entities.Helper;
using Stripe;

namespace MiHairCareApp.Configuration
{
    public static class StripeServiceExtension
    {
        public static void AddStripeConfiguration(this IServiceCollection services, IConfiguration config)
        {
            
            services.Configure<StripeSettings>(config.GetSection("Stripe"));
            StripeConfiguration.SetApiKey(config.GetSection("Stripe")["SecretKey"]);

        }
    }
}
