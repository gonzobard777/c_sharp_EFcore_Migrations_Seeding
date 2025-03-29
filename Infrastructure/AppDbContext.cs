using Domain.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }

    public AppDbContext()
    {
        Console.WriteLine("AppDbContext -> ctor AppDbContext()");
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Console.WriteLine("AppDbContext -> ctor AppDbContext(DbContextOptions<AppDbContext> options)");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        Console.WriteLine("AppDbContext -> OnConfiguring(DbContextOptionsBuilder options)");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Console.WriteLine("AppDbContext -> OnModelCreating(ModelBuilder modelBuilder)");
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Author");
            entity.HasKey(e => e.Id).HasName("PK.Author");
        });
    }
}