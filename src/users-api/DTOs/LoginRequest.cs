using System.ComponentModel.DataAnnotations;

namespace Prometheus.Users.Api.DTOs;

public sealed class LoginRequest
{
    [Required]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}
