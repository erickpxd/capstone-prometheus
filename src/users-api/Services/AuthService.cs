using Microsoft.EntityFrameworkCore;
using Prometheus.Users.Api.Data;
using Prometheus.Users.Api.Models;

namespace Prometheus.Users.Api.Services;

public sealed class AuthService(AppDbContext dbContext)
{
    public async Task<User?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Email == normalizedEmail, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return user;
    }

    public Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Id == id, cancellationToken);

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
