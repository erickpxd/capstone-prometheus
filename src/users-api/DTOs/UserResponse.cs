using Prometheus.Users.Api.Models;

namespace Prometheus.Users.Api.DTOs;

public sealed record UserResponse(
    int Id,
    string Name,
    string Email,
    UserRole Role,
    DateTimeOffset CreatedAt)
{
    public static UserResponse From(User user) =>
        new(user.Id, user.Name, user.Email, user.Role, user.CreatedAt);
}
