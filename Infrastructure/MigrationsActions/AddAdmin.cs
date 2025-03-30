using Domain.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.MigrationsActions;

public class AddAdmin : IMigrationAction<AppDbContext>
{
    public async Task BeforeMigration(AppDbContext dbContext)
    {
    }

    public async Task AfterMigration(AppDbContext dbContext)
    {
        var login = "admin";

        // var existedAdmin = dbContext.Database.SqlQuery<List<UserDbo>>(@$"
        //     SELECT 
        //         ""Id"", ""Login""
        //     FROM ""User"" as x
        // ");
        
        // var existedAdmin = dbContext.Database.SqlQuery<List<UserDbo>>(@$"
        //     SELECT 
        //         ""Id"", ""Login""
        //     FROM ""User"" as x
        // ").AsNoTracking().ToList();
        
        // var existedAdmin = dbContext.Database.SqlQueryRaw<UserDbo>(@$"
        //     SELECT 
        //         ""Id"", ""Login""
        //     FROM ""User"" as x
        //     WHERE x.""Login"" LIKE '%{login}%'
        // ").AsNoTracking().ToList();

        
        
        // Console.WriteLine();
        
        // var existedAdmin = dbContext
        //     .Users.AsNoTracking()
        //     .FirstOrDefault(user => user.Login == "admin");
        // if (existedAdmin == null)
        // {
        //     // Создать
        //     var admin = new User { Login = "admin", Salt = "Create" };
        //     dbContext.Users.Add(admin);
        //     dbContext.SaveChanges();
        // }
        // else
        // {
        //     // Изменить
        //     existedAdmin.Salt = "Update";
        //     dbContext.SaveChanges();
        // }
    }
}

class UserDbo
{
    public int Id { get; set; }
    public string Login { get; set; }
}