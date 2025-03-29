using Domain.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Author> Authors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Console.WriteLine("AppDbContext.OnModelCreating");
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Author");
            entity.HasKey(e => e.Id).HasName("PK.Author");
        });
    }
}