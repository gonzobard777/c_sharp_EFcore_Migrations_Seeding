namespace WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Hosting;

public class Program
{
    // public static void Main(string[] args)
    // {
    //     var builder = WebApplication.CreateBuilder(args);
    //
    //     // Add services to the container.
    //
    //     builder.Services.AddControllers();
    //
    //     var app = builder.Build();
    //
    //     // Configure the HTTP request pipeline.
    //
    //     app.MapControllers();
    //
    //     app.Run();
    // }
    
    public static void Main(string[] args)
    {
        var host = WebHost
            .CreateDefaultBuilder(args)
            //.BuildDefault<Startup>(Container.Instance)
            .UseStartup<Startup>()
            .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
            .Build();
            
        host.Run();
    }

    
    // Миграции. EF Core uses this method at design time to access the DbContext
    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                webBuilder => webBuilder.UseStartup<Startup>());
}