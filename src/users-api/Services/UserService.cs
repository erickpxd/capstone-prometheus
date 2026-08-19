using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Prometheus.Users.Api.Data;
using Prometheus.Users.Api.DTOs;
using Prometheus.Users.Api.Models;

namespace Prometheus.Users.Api.Services;

public enum CreateUserError
{
    None,
    InvalidName,
    InvalidRole,
    DuplicateEmail
}

public sealed record CreateUserResult(UserResponse? User, CreateUserError Error);

public sealed class UserService(AppDbContext dbContext)
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Name)
            .ThenBy(user => user.Id)
            .Select(user => new UserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<CreateUserResult> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new CreateUserResult(null, CreateUserError.InvalidName);
        }

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role) ||
            role is not (UserRole.Aluno or UserRole.Professor))
        {
            return new CreateUserResult(null, CreateUserError.InvalidRole);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var alreadyExists = await dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (alreadyExists)
        {
            return new CreateUserResult(null, CreateUserError.DuplicateEmail);
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is SqliteException { SqliteErrorCode: 19 })
        {
            return new CreateUserResult(null, CreateUserError.DuplicateEmail);
        }

        return new CreateUserResult(UserResponse.From(user), CreateUserError.None);
    }
}
