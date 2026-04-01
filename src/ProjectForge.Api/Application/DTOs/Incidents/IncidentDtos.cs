namespace ProjectForge.Api.Application.DTOs.Incidents;

public record CreateIncidentRequest(
    string Title,
    string? Description,
    string Severity = "Medium");

public record IncidentResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string Severity,
    string Status,
    DateTime CreatedAt);

// TODO: Add UpdateIncidentRequest when PATCH/PUT incident endpoints are implemented.
// TODO: Add ReporterId, AssigneeId to request/response when user tracking is added.
