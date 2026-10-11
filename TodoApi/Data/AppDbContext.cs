using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(user => user.Id);
            entity.HasIndex(user => user.UserName).IsUnique();
            entity.Property(user => user.UserName).HasMaxLength(64);
            entity.Property(user => user.Role).HasMaxLength(32);
        });

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(todo => todo.Id);
            entity.Property(todo => todo.Title).HasMaxLength(200);
        });
    }
}
