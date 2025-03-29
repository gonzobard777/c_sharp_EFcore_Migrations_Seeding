using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure;

public static class DbContextExtensions
{
    // namespace, к которому принадлежат все связанные действия для миграций.
    private const string ActionsNamespace = "Infrastructure.MigrationsActions";

    /// <summary>
    /// Последовательно накатывает на БД миграции, которые еще не были накачены,
    /// и ПОСЛЕ каждой миграции выполняет связанное с ней действие, если оно было предусмотрено.
    /// </summary>
    public static void MigrateAndAction(this AppDbContext dbContext)
    {
        Console.WriteLine("\n==========================");
        Console.WriteLine("=== Migrations Start");
        try
        {
            var migrator = dbContext.GetService<IMigrator>();
            var pendingMigrationNames = dbContext.Database.GetPendingMigrations().ToList();
            Console.WriteLine($"\nОтложенных миграций: {pendingMigrationNames.Count}шт.");
            foreach (var migrationName in pendingMigrationNames)
            {
                Console.WriteLine($"\n-- Start migration \"{migrationName}\" ------------");

                var actionTypeName = GetActionTypeName(migrationName);
                var action = GetAction(actionTypeName);

                // Действие перед миграцией.
                if (action != null)
                {
                    Console.WriteLine($"Run before action: {actionTypeName}");
                    action.BeforeMigration(dbContext);
                }

                // Накат миграции.
                Console.WriteLine($"Apply migration: \"{migrationName}\"");
                migrator.Migrate(migrationName);
                Console.WriteLine($"Done migration: \"{migrationName}\"");

                // Действие после миграции.
                if (action != null)
                {
                    Console.WriteLine($"Run after action: {actionTypeName}");
                    action.AfterMigration(dbContext);
                }

                Console.WriteLine($"-- End migration \"{migrationName}\" ------------\n");
            }

            // TODO: Действие после успешного выполнения всех миграций.
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка миграции");
            Console.WriteLine(e);
            throw;
        }

        Console.WriteLine("=== Migrations End");
        Console.WriteLine("==========================\n");
    }

    /// <summary>
    /// Действие - это класс, который:
    ///   1. Имеет название - actionName, совпадающее со смысловой частью названия миграции.
    ///      Смотри метод GetActionTypeName.
    ///   2. Его тип можно найти по actionTypeName: ActionsNamespace + "." + actionName
    ///      Смотри метод GetActionTypeName.
    ///   3. Реализует интерфейс IMigrationAction
    /// </summary>
    private static IMigrationAction<AppDbContext>? GetAction(string? actionTypeName)
    {
        if (actionTypeName == null) return null;
        var actionType = typeof(DbContextExtensions).Assembly.GetType(actionTypeName);
        if (
            actionType != null &&
            actionType.GetInterfaces().Any(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMigrationAction<>))
        )
            return Activator.CreateInstance(actionType) as IMigrationAction<AppDbContext>;

        return null;
    }

    /// <summary>
    /// Получить имя типа Действия.
    /// Ожидается, что миграция имеет название вида: 20250329195523_Init
    /// Соответственно смысловая часть здесь "Init" - это и есть actionName.
    /// А имя типа получается прибавлением namespace.
    /// </summary>
    private static string? GetActionTypeName(string? migrationName)
    {
        if (migrationName == null) return null;
        var actionName = migrationName.Substring(migrationName.IndexOf('_') + 1);
        if (actionName.Length == 0) return null;
        return ActionsNamespace + "." + actionName;
    }
}

public interface IMigrationAction<TDbContext> where TDbContext : DbContext
{
    void BeforeMigration(TDbContext dbContext);
    void AfterMigration(TDbContext dbContext);
}