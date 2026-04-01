namespace ProjectForge.Api.Application.DTOs.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token, string Email, string Role);
