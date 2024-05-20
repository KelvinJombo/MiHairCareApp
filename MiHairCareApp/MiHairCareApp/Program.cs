using MiHairCareApp.AutoMapper;
using MiHairCareApp.Commons;
using MiHairCareApp.Commons.Utilities;
using MiHairCareApp.Configuration;
using MiHairCareApp.Persistence.Extensions;
using NLog;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
ConfigurationHelper.InstantiateConfiguration(builder.Configuration);
var configuration = builder.Configuration;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    // Add services to the container.
    builder.Services.AddDependencies(configuration);
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAuthentication();

    builder.Services.ConfigureAuthentication(configuration);
    builder.Services.AddMailService(configuration);
    builder.Services.AddAutoMapper(typeof(MapperProfiles));
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        await Seeder.SeedRolesAndSuperAdmin(serviceProvider);
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

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
