//csharp MiHairCareApp.Persistence / Extensions / DIExtensions.cs
using CloudinaryDotNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Application.ServicesImplementation;
using MiHairCareApp.Commons.Utilities;
using MiHairCareApp.Domain.Entities.Helper;
using MiHairCareApp.Persistence.Repositories;

namespace MiHairCareApp.Persistence.Extensions
{
    public static class DIExtension
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Cloudinary settings using options pattern
            services.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            services.AddSingleton(provider =>
            {
                var settings = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
                var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                return new Cloudinary(account);
            });

            // Register services and repositories
            services.AddTransient<IEmailServices, EmailServices>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IHairStyleServices, HairStyleServices>();
            services.AddScoped<IAuthenticationServices, AuthenticationServices>();
            services.AddScoped<IStylistAuthServices, StylistAuthServices>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IReviewsService, ReviewsService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IReferralService, ReferralService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IWalletServices, WalletServices>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(ICloudinaryServices<>), typeof(CloudinaryServices<>));
        }
    }
}