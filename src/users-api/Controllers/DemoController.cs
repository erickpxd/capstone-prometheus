using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Prometheus.Users.Api.Controllers;

[ApiController]
[Route("api/demo")]
public sealed class DemoController : ControllerBase
{
    [Authorize]
    [HttpGet("authenticated")]
    public IActionResult Authenticated() =>
        Ok(new { message = "Acesso autorizado para usuário autenticado." });

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult Admin() =>
        Ok(new { message = "Acesso autorizado para Admin." });

    [Authorize(Roles = "Professor")]
    [HttpGet("professor")]
    public IActionResult Professor() =>
        Ok(new { message = "Acesso autorizado para Professor." });

    [Authorize(Roles = "Aluno")]
    [HttpGet("aluno")]
    public IActionResult Aluno() =>
        Ok(new { message = "Acesso autorizado para Aluno." });
}
