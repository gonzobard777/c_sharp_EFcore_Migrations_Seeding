using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure;

public static class AppDbContextExtensions
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
        Console.WriteLine("=== Migrations Start\n");
        try
        {
            var migrator = dbContext.GetService<IMigrator>();
            var pendingMigrationNames = dbContext.Database.GetPendingMigrations().ToList();
            Console.WriteLine($"\nОтложенных миграций: {pendingMigrationNames.Count}\n");
            for (int i = 0; i < pendingMigrationNames.Count; i++)
            {
                var count = i + 1;
                var migrationName = pendingMigrationNames[i]; // ожидается формат вида: "20250329195523_Init"

                Console.WriteLine($"-- [{count}] Start migration \"{migrationName}\" ------------");

                var actionTypeName = GetActionTypeName(migrationName);
                var action = GetAction(actionTypeName);

                // Действие перед миграцией.
                if (action != null)
                {
                    Console.WriteLine($"\nДействие ПЕРЕД миграцией: {actionTypeName}\n");
                    action.BeforeMigration(dbContext);
                }

                // Накат миграции.
                Console.WriteLine($"Накат миграции: \"{migrationName}\"\n");
                migrator.Migrate(migrationName);
                Console.WriteLine($"\nУспешно накатилась миграция: \"{migrationName}\"\n");

                // Действие после миграции.
                if (action != null)
                {
                    try
                    {
                        Console.WriteLine($"Действие ПОСЛЕ миграции: {actionTypeName}\n");
                        action.AfterMigration(dbContext);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("\nОшибка действия ПОСЛЕ миграции\n");
                        var successfulMigrations = dbContext.Database.GetAppliedMigrations().ToList();
                        // Если миграция успешно накатилась.
                        // Вообще говоря, миграция точно накатилась, но проверить на всякий случай надо.
                        if (migrationName == successfulMigrations.LastOrDefault())
                        {
                            // Если упало после самой первой миграции.
                            var secondAtLast = successfulMigrations.ElementAtOrDefault(successfulMigrations.Count - 2);
                            if (secondAtLast == null) // только у первой миграции нет предыдущей миграции
                            {
                                Console.WriteLine("\nОткат на состояние БД без миграций\n");
                                migrator.Migrate(Migration.InitialDatabase);
                            }
                            else
                            {
                                // Чтобы следующий раз эта миграция снова накатались
                                // и самое главное действие снова попыталось выполниться,
                                // ведь упало на действии.
                                Console.WriteLine($"\nОткат на предыдущую миграцию: {secondAtLast}\n");
                                migrator.Migrate(secondAtLast);
                            }
                        }

                        throw;
                    }
                }

                Console.WriteLine($"-- [{count}] End migration \"{migrationName}\" ------------\n");
            }

            // TODO: Действие после успешного выполнения всех миграций.
        }
        catch (Exception e)
        {
            Console.WriteLine("\nОшибка либо в миграции, либо в действии\n");
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
        var actionType = typeof(AppDbContextExtensions).Assembly.GetType(actionTypeName);
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