using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using MiHairCareApp.Persistence.Context;
using System.IO;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StylistsDBContext>
{
    public StylistsDBContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<StylistsDBContext>();
        var connectionString = configuration.GetConnectionString("StylistsConnection");
        optionsBuilder.UseSqlServer(connectionString);

        return new StylistsDBContext(optionsBuilder.Options);
    }
}

