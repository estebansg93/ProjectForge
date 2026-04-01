namespace ProjectForge.Api.Domain.Entities;

public class Incident
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Severity values: "Low" | "Medium" | "High" | "Critical"
    /// TODO: Replace with an enum.
    /// </summary>
    public string Severity { get; set; } = "Medium";

    /// <summary>
    /// Status values: "Open" | "Investigating" | "Resolved" | "Closed"
    /// TODO: Replace with an enum.
    /// </summary>
    public string Status { get; set; } = "Open";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Project Project { get; set; } = null!;

    // TODO: Add ReporterId, AssigneeId (FK to User).
    // TODO: Add ResolvedAt, ClosedAt timestamps.
    // TODO: Add timeline / activity log support.
}
