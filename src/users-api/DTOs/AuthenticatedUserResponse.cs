using Prometheus.Users.Api.Models;

namespace Prometheus.Users.Api.DTOs;

public sealed record AuthenticatedUserResponse(
    int Id,
    string Name,
    string Email,
    UserRole Role)
{
    public static AuthenticatedUserResponse From(User user) =>
        new(user.Id, user.Name, user.Email, user.Role);
}
