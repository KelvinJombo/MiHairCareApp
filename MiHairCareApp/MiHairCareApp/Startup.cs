using MiHairCareApp.AutoMapper;
using MiHairCareApp.Commons.Utilities;
using MiHairCareApp.Configuration;
using MiHairCareApp.Persistence.Extensions;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDependencies(Configuration);
        services.AddControllers();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddAuthentication();

        services.ConfigureAuthentication(Configuration);
        services.AddMailService(Configuration);
        services.AddAutoMapper(typeof(MapperProfiles));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        using (var scope = app.ApplicationServices.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            Seeder.SeedRolesAndSuperAdmin(serviceProvider).Wait();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
