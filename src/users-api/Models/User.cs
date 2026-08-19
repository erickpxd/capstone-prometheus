namespace Prometheus.Users.Api.Models;

public sealed class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
