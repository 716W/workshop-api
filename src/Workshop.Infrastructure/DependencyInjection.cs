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
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Audit interceptor (stateless singleton) ───────────────────────────
        services.AddSingleton<AuditableEntityInterceptor>();

        // ── EF Core – Pomelo MySQL ────────────────────────────────────────────
        services.AddDbContext<WorkshopDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' is not configured.");

            options.UseMySql(
                connectionString,
                new MySqlServerVersion(new Version(8, 0, 36)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null));

            options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
        });

        // ── Repositories ──────────────────────────────────────────────────────
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
