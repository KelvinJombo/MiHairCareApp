using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiHairCareApp.Application.Interfaces;
using MiHairCareApp.Application.Interfaces.Repository;
using MiHairCareApp.Application.Interfaces.Services;
using MiHairCareApp.Application.ServicesImplementation;
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
            services.AddDbContext<StylistsDBContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("StylistsConnection")));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<StylistsDBContext>()
                .AddDefaultTokenProviders();

            services.AddIdentity<Stylist, IdentityRole>()
                .AddEntityFrameworkStores<StylistsDBContext>()
                .AddDefaultTokenProviders();

            var emailSettings = new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(emailSettings);
            services.AddSingleton(emailSettings);

            services.AddTransient<IEmailServices, EmailServices>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IWalletServices, WalletServices>();
            services.AddScoped<IAuthenticationServices, AuthenticationServices>();
            services.AddScoped<IStylistServices, StylistServices>();
        }
    }

}
