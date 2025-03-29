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
        Console.WriteLine("==========================");
        Console.WriteLine("=== Migrations Start");
        try
        {
            var migrator = dbContext.GetService<IMigrator>();
            var pendingMigrationNames = dbContext.Database.GetPendingMigrations().ToList();
            Console.WriteLine($"Отложенных миграций: {pendingMigrationNames.Count}шт.");
            foreach (var name in pendingMigrationNames)
            {
                var action = GetAction(name);

                // Действие перед миграцией.
                action?.BeforeMigration(dbContext);

                // Накат миграции.
                Console.WriteLine($"apply: \"{name}\"");
                migrator.Migrate(name);
                Console.WriteLine($"done: \"{name}\"");

                // Действие после миграции.
                action?.AfterMigration(dbContext);
            }

            // TODO: Действие после успешного выполнения всех миграций и связанных действий.
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

    private static IMigrationAction<AppDbContext>? GetAction(string? migrationName)
    {
        IMigrationAction<AppDbContext>? action = null;
        if (migrationName == null) return action;

        var actionName = migrationName.Substring(migrationName.IndexOf('_') + 1);
        var actionType = typeof(DbContextExtensions).Assembly.GetType(ActionsNamespace + "." + actionName);
        if (
            actionType != null &&
            actionType.GetInterfaces().Any(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IMigrationAction<>))
        )
            action = Activator.CreateInstance(actionType) as IMigrationAction<AppDbContext>;

        return action;
    }
}

public interface IMigrationAction<TDbContext> where TDbContext : DbContext
{
    void BeforeMigration(TDbContext dbContext);
    void AfterMigration(TDbContext dbContext);
}