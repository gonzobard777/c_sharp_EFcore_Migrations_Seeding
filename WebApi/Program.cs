using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        var host = WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();

        // Миграции.
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            using (var dbContext = services.GetRequiredService<AppDbContext>())
            {
                try
                {
                    dbContext.Database.Migrate();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка миграции");
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        host.Run();
    }

    // Миграции. EF Core uses this method at design time to access the DbContext
    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                webBuilder => webBuilder.UseStartup<Startup>());
}