using Microondas.Api.Authentication;
using Microondas.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Microondas.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly AuthCredentials _credentials;

    public AuthController(JwtTokenGenerator tokenGenerator, AuthCredentials credentials)
    {
        _tokenGenerator = tokenGenerator;
        _credentials = credentials;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var hashedPassword = PasswordHasher.HashSha256(request.Password);

        if (request.Username != _credentials.Username || hashedPassword != _credentials.HashedPassword)
            return Unauthorized(new { message = "Credenciais inválidas." });

        var token = _tokenGenerator.Generate(request.Username);
        return Ok(new { token, expiresIn = 3600 });
    }
}

public sealed record LoginRequest(string Username, string Password);