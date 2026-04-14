namespace ProjectForge.Api.Application.DTOs.Tasks;

public record CreateTaskRequest(
    string Title,
    string? Description,
    string Priority = "Medium");

public record TaskResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime CreatedAt);

public record UpdateTaskRequest(
    string Title,
    string? Description,
    string Status,
    string Priority);

// TODO: Add AssigneeId to CreateTaskRequest when user assignment is supported.
