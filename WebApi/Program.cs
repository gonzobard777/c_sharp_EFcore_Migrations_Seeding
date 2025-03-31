using Microsoft.AspNetCore;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"-   WebApi -> run Main(args[{args.Length}]): {string.Join(",", args.ToArray())}");

        var host = WebHost
            .CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();

        host.Run();
    }

    // Метод вызывается при выполнении консольной команды добавления миграции:
    //  dotnet ef migrations add ...
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        Console.WriteLine($"-   WebApi -> run CreateHostBuilder(args[{args.Length}]): {string.Join(",", args.ToArray())}");

        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(
                webBuilder => webBuilder.UseStartup<Startup>());
    }
}