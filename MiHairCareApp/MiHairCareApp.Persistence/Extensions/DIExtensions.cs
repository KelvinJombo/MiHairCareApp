using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Application.ServicesImplementation;
using MiHairCareApp.Commons.Utilities;
using MiHairCareApp.Domain.Entities;
using MiHairCareApp.Domain.Entities.Helper;
using MiHairCareApp.Persistence.Context;
using MiHairCareApp.Persistence.Repositories;

namespace MiHairCareApp.Persistence.Extensions
{
    public static class DIExtensions
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Register the DbContext
            services.AddDbContext<StylistsDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("StylistsConnection")));

            // Register Identity services for AppUser
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<StylistsDBContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication().AddGoogle(googleOptions =>
            {
                googleOptions.ClientId = configuration["Authentication:Google:ClientId"]!;
                googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            });

            // Register UserManager and RoleManager for both AppUser and Stylist
            services.AddScoped<UserManager<AppUser>>();             
            services.AddScoped<RoleManager<IdentityRole>>();

            // Register other services
            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            services.AddSingleton(emailSettings);

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


            // Register Cloudinary services
            var cloudinarySettings = new CloudinarySettings();
            configuration.GetSection("CloudinarySettings").Bind(cloudinarySettings);
            services.AddSingleton(cloudinarySettings);
            services.AddSingleton(provider =>
            {
                var account = new Account(
                    cloudinarySettings.CloudName,
                    cloudinarySettings.ApiKey,
                    cloudinarySettings.ApiSecret);
                return new Cloudinary(account);
            });

             
            services.AddScoped(typeof(ICloudinaryServices<>), typeof(CloudinaryServices<>));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
 
            services.AddScoped(typeof(CloudinaryServices<>));

        } 


    }
}