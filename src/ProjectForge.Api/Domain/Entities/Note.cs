namespace ProjectForge.Api.Domain.Entities;

public class Note
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Project Project { get; set; } = null!;

    // TODO: Add AuthorId (FK to User) when note authorship is introduced.
    // TODO: Add Title, UpdatedAt fields.
    // TODO: Consider soft delete (IsDeleted flag) for note history.
}
