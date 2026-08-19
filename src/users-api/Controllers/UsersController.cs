using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Users.Api.DTOs;
using Prometheus.Users.Api.Services;

namespace Prometheus.Users.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/users")]
public sealed class UsersController(UserService userService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var users = await userService.GetAllAsync(cancellationToken);
        return Ok(users);
    }

    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userService.CreateAsync(request, cancellationToken);

        if (result.Error == CreateUserError.InvalidName)
        {
            ModelState.AddModelError(nameof(request.Name), "O nome é obrigatório.");
            return ValidationProblem(ModelState);
        }

        if (result.Error == CreateUserError.InvalidRole)
        {
            ModelState.AddModelError(
                nameof(request.Role),
                "O perfil deve ser Aluno ou Professor.");
            return ValidationProblem(ModelState);
        }

        if (result.Error == CreateUserError.DuplicateEmail)
        {
            return Conflict(new { message = "Já existe um usuário com este e-mail." });
        }

        return StatusCode(StatusCodes.Status201Created, result.User);
    }
}
