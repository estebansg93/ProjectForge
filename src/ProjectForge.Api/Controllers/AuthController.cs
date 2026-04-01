using Microsoft.AspNetCore.Mvc;
using ProjectForge.Api.Application.DTOs.Auth;
using ProjectForge.Api.Application.Interfaces;

namespace ProjectForge.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Authenticates a user and returns a JWT bearer token.
    /// </summary>
    /// <remarks>
    /// Demo credentials for local development:
    /// - Admin: admin@projectforge.local / Admin1234!
    /// - Member: member@projectforge.local / Member1234!
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);

        // TODO: Add POST /api/auth/register endpoint.
        // TODO: Add POST /api/auth/refresh endpoint for refresh token support.
        // TODO: Add POST /api/auth/logout endpoint for token revocation.
        // TODO: Add POST /api/auth/forgot-password endpoint.
    }
}
