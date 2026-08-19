using Microsoft.EntityFrameworkCore;
using Prometheus.Users.Api.Models;

namespace Prometheus.Users.Api.Data;

public static class DatabaseInitializer
{
    public const string DemonstrationAdminEmail = "admin@prometheus.local";
    public const string DemonstrationAdminPassword = "Admin@123";

    public static async Task InitializeAsync(
        IServiceProvider services,
        bool seedDemonstrationAdmin,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);

        if (!seedDemonstrationAdmin)
        {
            return;
        }

        var adminExists = await dbContext.Users
            .AnyAsync(user => user.Email == DemonstrationAdminEmail, cancellationToken);

        if (adminExists)
        {
            return;
        }

        dbContext.Users.Add(new User
        {
            Name = "Administrador",
            Email = DemonstrationAdminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemonstrationAdminPassword),
            Role = UserRole.Admin,
            CreatedAt = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
