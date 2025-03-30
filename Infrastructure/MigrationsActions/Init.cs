namespace Infrastructure.MigrationsActions;

public class Init : IMigrationAction<AppDbContext>
{
    public async Task BeforeMigration(AppDbContext dbContext)
    {
        Console.WriteLine("  BeforeMigration - Init action");
    }

    public async Task AfterMigration(AppDbContext dbContext)
    {
        Console.WriteLine("  AfterMigration - Init action");
    }
}