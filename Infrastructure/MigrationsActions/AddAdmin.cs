using Microsoft.EntityFrameworkCore;

namespace Infrastructure.MigrationsActions;

public class AddAdmin : IMigrationAction<AppDbContext>
{
    private const string AdminLogin = "admin";

    public void BeforeMigration(AppDbContext dbContext)
    {
    }

    public void AfterMigration(AppDbContext dbContext)
    {
        // var existedAdmin = dbContext
        //     .Users.AsNoTracking()
        //     .FirstOrDefault(x => x.Login == AdminLogin);

        var existedAdmin = dbContext.Database.SqlQueryRaw<UserDbo>(@$"
            SELECT ""Id""
            FROM ""User"" as x
            WHERE x.""Login"" LIKE '%{AdminLogin}%'
        ").AsNoTracking().FirstOrDefault();

        if (existedAdmin == null)
        {
            // Создать.
            dbContext.Database.ExecuteSqlRaw(@$"
                INSERT INTO ""User"" VALUES 
                (DEFAULT, '{AdminLogin}', 'by Creating')
            ");
        }
        else
        {
            // Изменить.
            dbContext.Database.ExecuteSqlRaw($@"
                UPDATE ""User"" 
                SET ""Salt"" = 'by Udating' 
                WHERE ""Id"" = {existedAdmin.Id}
            ");
        }
    }
}

class UserDbo
{
    public int Id { get; set; }
    // public string Login { get; set; }
    // public string Salt { get; set; }
}