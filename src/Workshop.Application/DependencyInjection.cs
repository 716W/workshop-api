using Microsoft.Extensions.DependencyInjection;
using Workshop.Application.Interfaces;
using Workshop.Application.Services;

namespace Workshop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IJobCardService, JobCardService>();
        services.AddScoped<IInventoryService, InventoryService>();

        return services;
    }
}
