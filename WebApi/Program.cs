using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"-   WebApi -> run Main(args[{args.Length}]): {string.Join(",", args.ToArray())}");

        var host = WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();

        // Миграции.
        Console.WriteLine("==========================");
        Console.WriteLine("=== Migrations Start");
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            using (var dbContext = services.GetRequiredService<AppDbContext>())
            {
                try
                {
                    var migrator = dbContext.GetService<IMigrator>();
                    var pendingMigrationNames = dbContext.Database.GetPendingMigrations().ToList();
                    Console.WriteLine($"Отложенных миграций: {pendingMigrationNames.Count}шт.");
                    foreach (var name in pendingMigrationNames)
                    {
                        // TODO: Do things before each migration.
                        Console.WriteLine($"start Migration -> {name}");
                        migrator.Migrate(name);
                        // TODO: Do things after each migration.
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка миграции");
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        Console.WriteLine("=== Migrations End");
        Console.WriteLine("==========================");


        host.Run();
    }

    // Миграции. EF Core uses this method at design time to access the DbContext
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        Console.WriteLine($"-   WebApi -> run CreateHostBuilder(args[{args.Length}]): {string.Join(",", args.ToArray())}");

        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                webBuilder => webBuilder.UseStartup<Startup>());
    }
}