namespace Prometheus.Users.Api.DTOs;

public sealed record LoginResponse(
    string Token,
    DateTimeOffset ExpiresAt,
    AuthenticatedUserResponse User);
