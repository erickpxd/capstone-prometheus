using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Users.Api.DTOs;
using Prometheus.Users.Api.Services;

namespace Prometheus.Users.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService authService, JwtService jwtService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await authService.AuthenticateAsync(
            request.Email,
            request.Password,
            cancellationToken);

        if (user is null)
        {
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        var token = jwtService.CreateToken(user);
        return Ok(new LoginResponse(
            token.Token,
            token.ExpiresAt,
            AuthenticatedUserResponse.From(user)));
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType<AuthenticatedUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticatedUserResponse>> Me(
        CancellationToken cancellationToken)
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!int.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await authService.FindByIdAsync(userId, cancellationToken);
        return user is null
            ? Unauthorized()
            : Ok(AuthenticatedUserResponse.From(user));
    }
}
