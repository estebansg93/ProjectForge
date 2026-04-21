using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Projects;
using ProjectForge.Api.Application.Services;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Tests;

public class ProjectServiceTests
{
    private static DbContextOptions<AppDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private static AppDbContext CreateDb() => new(CreateOptions());

    private static Project SeedProject(AppDbContext db, Guid id, string name = "Test Project", string status = "Active")
    {
        var project = new Project
        {
            Id = id,
            Name = name,
            Description = "Original description",
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
        db.Projects.Add(project);
        db.SaveChanges();
        return project;
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenProjectDoesNotExist()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);

        var result = await service.UpdateAsync(Guid.NewGuid(), new UpdateProjectRequest("New Name", null, "Active"));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesAllFields_WhenProjectExists()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        var id = Guid.NewGuid();
        SeedProject(db, id);

        var result = await service.UpdateAsync(id, new UpdateProjectRequest("Updated Name", "Updated desc", "Completed"));

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated desc", result.Description);
        Assert.Equal("Completed", result.Status);
    }

    [Fact]
    public async Task UpdateAsync_PreservesCreatedAt()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        var id = Guid.NewGuid();
        var seeded = SeedProject(db, id);
        var expectedCreatedAt = seeded.CreatedAt;

        var result = await service.UpdateAsync(id, new UpdateProjectRequest("New", null, "Active"));

        Assert.NotNull(result);
        Assert.Equal(expectedCreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges_ToDatabase()
    {
        var options = CreateOptions();
        var id = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedProject(db, id);
            var service = new ProjectService(db);
            await service.UpdateAsync(id, new UpdateProjectRequest("Persisted", "Desc", "Archived"));
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Projects.FindAsync(id);
            Assert.NotNull(stored);
            Assert.Equal("Persisted", stored.Name);
            Assert.Equal("Archived", stored.Status);
        }
    }

    [Fact]
    public async Task UpdateAsync_AllowsNullDescription()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        var id = Guid.NewGuid();
        SeedProject(db, id);

        var result = await service.UpdateAsync(id, new UpdateProjectRequest("Name", null, "Active"));

        Assert.NotNull(result);
        Assert.Null(result.Description);
    }

    // --- UpdateStatusAsync ---

    [Fact]
    public async Task UpdateStatusAsync_ReturnsNull_WhenProjectDoesNotExist()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);

        var result = await service.UpdateStatusAsync(Guid.NewGuid(), "Archived");

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_UpdatesStatus_WhenProjectExists()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        var id = Guid.NewGuid();
        SeedProject(db, id);

        var result = await service.UpdateStatusAsync(id, "Archived");

        Assert.NotNull(result);
        Assert.Equal("Archived", result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_PreservesOtherFields()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        var id = Guid.NewGuid();
        var seeded = SeedProject(db, id, name: "Keep Me");

        var result = await service.UpdateStatusAsync(id, "Completed");

        Assert.NotNull(result);
        Assert.Equal(seeded.Name, result.Name);
        Assert.Equal(seeded.Description, result.Description);
        Assert.Equal(seeded.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task UpdateStatusAsync_PersistsStatus_ToDatabase()
    {
        var options = CreateOptions();
        var id = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedProject(db, id);
            var service = new ProjectService(db);
            await service.UpdateStatusAsync(id, "Completed");
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Projects.FindAsync(id);
            Assert.NotNull(stored);
            Assert.Equal("Completed", stored.Status);
        }
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenProjectDoesNotExist()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);

        var result = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenProjectExists()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        var id = Guid.NewGuid();
        SeedProject(db, id);

        var result = await service.DeleteAsync(id);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProject_FromDatabase()
    {
        var options = CreateOptions();
        var id = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedProject(db, id);
            var service = new ProjectService(db);
            await service.DeleteAsync(id);
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Projects.FindAsync(id);
            Assert.Null(stored);
        }
    }
}
