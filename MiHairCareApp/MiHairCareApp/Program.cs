using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using MiHairCareApp.AutoMapper;
using MiHairCareApp.Commons;
using MiHairCareApp.Commons.Utilities;
using MiHairCareApp.Configuration;
using MiHairCareApp.Persistence.Extensions;
using NLog;
using NLog.Web;
using Stripe;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);
ConfigurationHelper.InstantiateConfiguration(builder.Configuration);
var configuration = builder.Configuration;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(builder.Environment.ContentRootPath, "firebase-adminsdk-fbsvc-8fba3f0b9e.json"))
});

try
{
    // Add services to the container.
    builder.Services.AddDependencies(configuration);
    builder.Services.AddControllers().AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;        
        
        opts.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        
    });

    StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

    // Ensure AddAuthentication is called only once
    builder.Services.ConfigureAuthentication(configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReactApp", policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://mihaircareapp.netlify.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });




    builder.Services.AddMailService(configuration);
    builder.Services.AddAutoMapper(typeof(MapperProfiles));

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter token",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    });


    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

     
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseDeveloperExceptionPage();
    }

     
    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseCors("AllowReactApp");

    app.UseAuthentication();
    app.UseAuthorization();

     
    app.MapControllers();

     
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;
        await Seeder.SeedRolesAndSuperAdmin(serviceProvider);
    }

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
