using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services, string? connStr)
    {
        PrintConnectionString(connStr);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connStr)
        );
        return services;
    }


    /// <summary>
    /// Распечатать строку подключения к БД.
    /// Постараться скрыть пароль.
    /// </summary>
    private static void PrintConnectionString(string? connStr)
    {
        void Print(string? printStr) => Console.WriteLine($"db connection: {printStr}");

        if (connStr == null)
        {
            Print("No connection string");
            return;
        }

        const string pattern = "password=";
        var printStr = connStr;
        var pos = connStr.IndexOf(pattern, StringComparison.Ordinal);
        if (pos >= 0)
        {
            pos += pattern.Length;
            var password = "";
            for (int i = pos; i < connStr.Length; i++)
            {
                var c = connStr[i];
                if (c == ';') // Пароль заканчивается символом ';'
                    break;
                password += c;
            }

            if (password.Length > 0)
                printStr = connStr.Replace(password, "***");
        }

        Print(printStr);
    }
}