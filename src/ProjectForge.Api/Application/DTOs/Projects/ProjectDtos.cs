namespace ProjectForge.Api.Application.DTOs.Projects;

public record CreateProjectRequest(string Name, string? Description);

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    DateTime CreatedAt);

// TODO: Add UpdateProjectRequest when PUT /api/projects/{id} is implemented.
// TODO: Add ProjectDetailResponse with task/note/incident counts.
