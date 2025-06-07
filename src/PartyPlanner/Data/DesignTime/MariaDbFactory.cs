// Data/DesignTime/MariaDbFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using PartyPlanner.Data;

namespace PartyPlanner.DesignTime;

public sealed class MariaDbFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // 1. Config inlezen (werkt ook als je vanuit solution-root runt)
        var basePath = Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(
                "appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        // 2. ServerVersion automatisch detecteren (handig bij upgrades)
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseMySql(connectionString, serverVersion,
                mySql => mySql
                    .EnableRetryOnFailure());

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
