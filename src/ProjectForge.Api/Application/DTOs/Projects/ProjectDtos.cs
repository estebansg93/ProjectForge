namespace ProjectForge.Api.Application.DTOs.Projects;

public record CreateProjectRequest(string Name, string? Description);

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    DateTime CreatedAt);

public record UpdateProjectRequest(string Name, string? Description, string Status);

public record PatchProjectStatusRequest(string Status);
