namespace ProjectForge.Api.Domain.Entities;

/// <summary>
/// Named ProjectTask to avoid collision with System.Threading.Tasks.Task.
/// The database table is mapped to "Tasks" via EF configuration.
/// </summary>
public class ProjectTask
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Status values: "Todo" | "InProgress" | "Done"
    /// TODO: Replace with an enum.
    /// </summary>
    public string Status { get; set; } = "Todo";

    /// <summary>
    /// Priority values: "Low" | "Medium" | "High"
    /// TODO: Replace with an enum.
    /// </summary>
    public string Priority { get; set; } = "Medium";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Project Project { get; set; } = null!;

    // TODO: Add AssigneeId (FK to User) when task assignment is introduced.
    // TODO: Add DueDate, CompletedAt fields.
}
