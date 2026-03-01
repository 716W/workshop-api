using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Workshop.Infrastructure.Data;

namespace Workshop.Infrastructure;

/// <summary>
/// Provides a WorkshopDbContext at EF Core design time (migrations, dotnet ef tools)
/// without requiring a running MySQL server. Uses the same connection string that
/// lives in appsettings.json / appsettings.Development.json.
/// </summary>
public class WorkshopDbContextFactory : IDesignTimeDbContextFactory<WorkshopDbContext>
{
    public WorkshopDbContext CreateDbContext(string[] args)
    {
        // Walk up from the Infrastructure bin directory to find the API's appsettings.
        var basePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "Workshop.API");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<WorkshopDbContext>();
        optionsBuilder.UseMySql(
            connectionString,
            new MySqlServerVersion(new Version(8, 0, 36)));

        return new WorkshopDbContext(optionsBuilder.Options);
    }
}
