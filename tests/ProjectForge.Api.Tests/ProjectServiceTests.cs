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

    private static Project SeedProject(AppDbContext db, Guid id, string name = "Test Project", string status = "Active",
        int taskCount = 0, int noteCount = 0, int incidentCount = 0)
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

        for (var i = 0; i < taskCount; i++)
            db.Tasks.Add(new ProjectTask { Id = Guid.NewGuid(), ProjectId = id, Title = $"Task {i}", Status = "Todo", Priority = "Low", CreatedAt = DateTime.UtcNow });

        for (var i = 0; i < noteCount; i++)
            db.Notes.Add(new Note { Id = Guid.NewGuid(), ProjectId = id, Content = $"Note {i}", CreatedAt = DateTime.UtcNow });

        for (var i = 0; i < incidentCount; i++)
            db.Incidents.Add(new Incident { Id = Guid.NewGuid(), ProjectId = id, Title = $"Incident {i}", Severity = "Low", Status = "Open", CreatedAt = DateTime.UtcNow });

        db.SaveChanges();
        return project;
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsAllProjects_WhenNoFilter()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        SeedProject(db, Guid.NewGuid(), "Alpha");
        SeedProject(db, Guid.NewGuid(), "Beta");

        var result = await service.GetAllAsync(page: 1, pageSize: 20, status: null);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_FiltersByStatus()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        SeedProject(db, Guid.NewGuid(), status: "Active");
        SeedProject(db, Guid.NewGuid(), status: "Archived");
        SeedProject(db, Guid.NewGuid(), status: "Active");

        var result = await service.GetAllAsync(page: 1, pageSize: 20, status: "Active");

        Assert.Equal(2, result.Count());
        Assert.All(result, r => Assert.Equal("Active", r.Status));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenStatusMatchesNone()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        SeedProject(db, Guid.NewGuid(), status: "Active");

        var result = await service.GetAllAsync(page: 1, pageSize: 20, status: "Archived");

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_PaginatesResults()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        for (var i = 0; i < 5; i++)
            SeedProject(db, Guid.NewGuid(), name: $"Project {i}");

        var page1 = await service.GetAllAsync(page: 1, pageSize: 2, status: null);
        var page2 = await service.GetAllAsync(page: 2, pageSize: 2, status: null);
        var page3 = await service.GetAllAsync(page: 3, pageSize: 2, status: null);

        Assert.Equal(2, page1.Count());
        Assert.Equal(2, page2.Count());
        Assert.Single(page3);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsEmpty_WhenPageBeyondResults()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        SeedProject(db, Guid.NewGuid());

        var result = await service.GetAllAsync(page: 99, pageSize: 20, status: null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_IncludesCorrectCounts()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        var id = Guid.NewGuid();
        SeedProject(db, id, taskCount: 3, noteCount: 2, incidentCount: 1);

        var result = (await service.GetAllAsync(page: 1, pageSize: 20, status: null)).Single();

        Assert.Equal(3, result.TaskCount);
        Assert.Equal(2, result.NoteCount);
        Assert.Equal(1, result.IncidentCount);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsZeroCounts_WhenNoRelatedData()
    {
        using var db = CreateDb();
        var service = new ProjectService(db);
        SeedProject(db, Guid.NewGuid());

        var result = (await service.GetAllAsync(page: 1, pageSize: 20, status: null)).Single();

        Assert.Equal(0, result.TaskCount);
        Assert.Equal(0, result.NoteCount);
        Assert.Equal(0, result.IncidentCount);
    }

    [Fact]
    public async Task GetAllAsync_OrdersByCreatedAtDescending()
    {
        var options = CreateOptions();
        using var db = new AppDbContext(options);
        var older = new Project { Id = Guid.NewGuid(), Name = "Older", Status = "Active", CreatedAt = DateTime.UtcNow.AddDays(-1) };
        var newer = new Project { Id = Guid.NewGuid(), Name = "Newer", Status = "Active", CreatedAt = DateTime.UtcNow };
        db.Projects.AddRange(older, newer);
        db.SaveChanges();
        var service = new ProjectService(db);

        var result = (await service.GetAllAsync(page: 1, pageSize: 20, status: null)).ToList();

        Assert.Equal("Newer", result[0].Name);
        Assert.Equal("Older", result[1].Name);
    }

    [Fact]
    public async Task GetAllAsync_IsStable_WhenCreatedAtTimestampsCollide()
    {
        var options = CreateOptions();
        using var db = new AppDbContext(options);
        var timestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var idA = new Guid("aaaaaaaa-0000-0000-0000-000000000000");
        var idB = new Guid("bbbbbbbb-0000-0000-0000-000000000000");
        db.Projects.AddRange(
            new Project { Id = idA, Name = "A", Status = "Active", CreatedAt = timestamp },
            new Project { Id = idB, Name = "B", Status = "Active", CreatedAt = timestamp });
        db.SaveChanges();
        var service = new ProjectService(db);

        var page1 = (await service.GetAllAsync(page: 1, pageSize: 1, status: null)).Single();
        var page2 = (await service.GetAllAsync(page: 2, pageSize: 1, status: null)).Single();

        Assert.NotEqual(page1.Id, page2.Id);
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
