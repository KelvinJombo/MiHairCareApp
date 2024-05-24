using MiHairCareApp.AutoMapper;
using MiHairCareApp.Commons;
using MiHairCareApp.Commons.Utilities;
using MiHairCareApp.Configuration;
using MiHairCareApp.Persistence.Extensions;
using NLog;
using NLog.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationHelper.InstantiateConfiguration(builder.Configuration);
var configuration = builder.Configuration;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    // Add services to the container.
    builder.Services.AddDependencies(configuration);
    builder.Services.AddControllers();

    

    // Ensure AddAuthentication is called only once
    builder.Services.ConfigureAuthentication(configuration);
    builder.Services.AddMailService(configuration);
    builder.Services.AddAutoMapper(typeof(MapperProfiles));

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        await Seeder.SeedRolesAndSuperAdmin(serviceProvider);
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthentication(); // Ensure UseAuthentication is called only once
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    });

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Error encountered in the middleware pipeline");
    throw; // Re-throw the exception to allow further handling if necessary
}
finally
{
    NLog.LogManager.Shutdown();
}
