using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Workshop.Domain.Interfaces;
using Workshop.Infrastructure.Data;
using Workshop.Infrastructure.Interceptors;
using Workshop.Infrastructure.Repositories;

namespace Workshop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the audit interceptor as a singleton (stateless – safe to share).
        services.AddSingleton<AuditableEntityInterceptor>();

        // EF Core – inject the interceptor via the options builder.
        services.AddDbContext<WorkshopDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32)));
            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
