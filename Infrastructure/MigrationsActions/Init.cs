namespace Infrastructure.MigrationsActions;

public class Init : IMigrationAction<AppDbContext>
{
    public void BeforeMigration(AppDbContext dbContext)
    {
        Console.WriteLine("  BeforeMigration - Init action");
    }

    public void AfterMigration(AppDbContext dbContext)
    {
        Console.WriteLine("  AfterMigration - Init action");
    }
}