namespace ProjectForge.Api.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Role values: "Admin" | "Member"
    /// TODO: Replace with an enum or a proper role system if requirements grow.
    /// </summary>
    public string Role { get; set; } = "Member";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // TODO: Add navigation properties if user-owned resources are introduced later
    // (e.g., tasks assigned to a user, projects owned by a user).
}
