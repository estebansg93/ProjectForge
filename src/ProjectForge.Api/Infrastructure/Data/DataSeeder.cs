using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Domain.Entities;

namespace ProjectForge.Api.Infrastructure.Data;

/// <summary>
/// Seeds baseline data required for local development.
/// Runs at application startup only when data is missing.
/// 
/// TODO: Replace with a more robust seeding strategy (e.g., separate CLI tool,
///       idempotent migration seeding, or a dedicated /admin/seed endpoint for dev).
/// TODO: Move credentials to environment variables or dev secrets.
/// TODO: Add seed data for sample Tasks, Notes, and Incidents.
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        await SeedUsersAsync(context);
        await SeedProjectsAsync(context);
    }

    private static async Task SeedUsersAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Email = "admin@projectforge.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Email = "member@projectforge.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Member1234!"),
                Role = "Member",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProjectsAsync(AppDbContext context)
    {
        if (await context.Projects.AnyAsync())
            return;

        var projects = new List<Project>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Platform Infrastructure",
                Description = "Core infrastructure work for the platform.",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mobile App v2",
                Description = "Second major iteration of the mobile client.",
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Projects.AddRange(projects);
        await context.SaveChangesAsync();
    }
}
