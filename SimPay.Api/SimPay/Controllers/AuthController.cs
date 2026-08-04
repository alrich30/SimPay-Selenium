using Microsoft.AspNetCore.Mvc;

namespace SimPay.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private const string ValidUsername = "admin";
    private const string ValidPassword = "SimPay123!";

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "El usuario y la contraseña son obligatorios."
            });
        }

        if (request.Username != ValidUsername ||
            request.Password != ValidPassword)
        {
            return Unauthorized(new
            {
                message = "Credenciales incorrectas."
            });
        }

        return Ok(new
        {
            message = "Inicio de sesión exitoso.",
            username = request.Username,
            accessToken = "simulated-access-token"
        });
    }
}

public sealed record LoginRequest(
    string? Username,
    string? Password);