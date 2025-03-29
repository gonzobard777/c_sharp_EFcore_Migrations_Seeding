using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure;

public static class DbContextExtensions
{
    /// <summary>
    /// Последовательно накатывает на БД миграции, которые еще не были накачены,
    /// и ПОСЛЕ каждой миграции выполняет связанное с ней действие, если оно было предусмотрено.
    /// </summary>
    public static void MigrateAndAction(this DbContext dbContext)
    {
        Console.WriteLine("==========================");
        Console.WriteLine("=== Migrations Start");
        try
        {
            var migrator = dbContext.GetService<IMigrator>();
            var pendingMigrationNames = dbContext.Database.GetPendingMigrations().ToList();
            Console.WriteLine($"Отложенных миграций: {pendingMigrationNames.Count}шт.");
            foreach (var name in pendingMigrationNames)
            {
                // TODO: Действия перед конкретной миграцией.
                Console.WriteLine($"start: \"{name}\"");
                migrator.Migrate(name);
                Console.WriteLine($"done: \"{name}\"");
                // TODO: Действия после конкретной миграции.
            }
            // TODO: Действия после всех миграций.
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка миграции");
            Console.WriteLine(e);
            throw;
        }

        Console.WriteLine("=== Migrations End");
        Console.WriteLine("==========================");
    }
}