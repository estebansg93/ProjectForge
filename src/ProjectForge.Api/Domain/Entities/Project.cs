namespace ProjectForge.Api.Domain.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Status values: "Active" | "Archived" | "Completed"
    /// TODO: Replace with a strongly-typed enum once statuses are finalised.
    /// </summary>
    public string Status { get; set; } = "Active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<Note> Notes { get; set; } = new List<Note>();
    public ICollection<Incident> Incidents { get; set; } = new List<Incident>();

    // TODO: Add OwnerId (FK to User) when user-project ownership is introduced.
}
