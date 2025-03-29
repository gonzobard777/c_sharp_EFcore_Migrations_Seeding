using MapMakers.GMOnline.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, string connStr)
    {
        Console.WriteLine($"db.UseNpgsql: {connStr}");
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connStr)
        );
        return services;
    }
}