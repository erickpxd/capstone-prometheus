using System.ComponentModel.DataAnnotations;

namespace Prometheus.Users.Api.DTOs;

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string Password { get; init; } = string.Empty;

    [Required]
    public string Role { get; init; } = string.Empty;
}
