using Microsoft.EntityFrameworkCore;
using Prometheus.Users.Api.Models;

namespace Prometheus.Users.Api.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();

        user.ToTable("Users");
        user.HasKey(item => item.Id);
        user.Property(item => item.Name).IsRequired().HasMaxLength(120);
        user.Property(item => item.Email).IsRequired().HasMaxLength(254);
        user.Property(item => item.PasswordHash).IsRequired();
        user.Property(item => item.Role).IsRequired();
        user.Property(item => item.CreatedAt).IsRequired();
        user.HasIndex(item => item.Email).IsUnique();
    }
}
