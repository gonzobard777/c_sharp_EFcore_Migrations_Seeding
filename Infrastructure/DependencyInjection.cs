using MapMakers.GMOnline.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration[ConnectionStrings.DBConnectionString];
        Console.WriteLine($"db: {connectionString}");
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );
        return services;
    }
}