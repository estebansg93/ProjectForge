using ProjectForge.Api.Application.DTOs.Auth;

namespace ProjectForge.Api.Application.Interfaces;

public interface IAuthService
{
    /// <summary>
    /// Validates credentials and returns a JWT token on success.
    /// Returns null if credentials are invalid.
    /// </summary>
    Task<LoginResponse?> LoginAsync(LoginRequest request);

    // TODO: Task<bool> RegisterAsync(RegisterRequest request);
    // TODO: Task<LoginResponse?> RefreshTokenAsync(string refreshToken);
    // TODO: Task RevokeTokenAsync(string userId);
}
