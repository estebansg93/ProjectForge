using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Tasks;
using ProjectForge.Api.Application.Services;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Tests;

public class TaskServiceTests
{
    private static DbContextOptions<AppDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private static AppDbContext CreateDb() => new(CreateOptions());

    private static ProjectTask SeedTask(AppDbContext db, Guid projectId, Guid taskId,
        string status = "Todo", string priority = "Low")
    {
        var task = new ProjectTask
        {
            Id = taskId,
            ProjectId = projectId,
            Title = "Original Title",
            Description = "Original description",
            Status = status,
            Priority = priority,
            CreatedAt = DateTime.UtcNow
        };
        db.Tasks.Add(task);
        db.SaveChanges();
        return task;
    }

    // --- GetByProjectAsync ---

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_ReturnsAllTasks_WhenNoFilter()
    {
        using var db = CreateDb();
        var service = new TaskService(db);
        var projectId = Guid.NewGuid();
        SeedTask(db, projectId, Guid.NewGuid());
        SeedTask(db, projectId, Guid.NewGuid());

        var result = await service.GetByProjectAsync(projectId, page: 1, pageSize: 20, status: null, priority: null);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_FiltersByStatus()
    {
        using var db = CreateDb();
        var service = new TaskService(db);
        var projectId = Guid.NewGuid();
        SeedTask(db, projectId, Guid.NewGuid(), status: "Todo");
        SeedTask(db, projectId, Guid.NewGuid(), status: "InProgress");
        SeedTask(db, projectId, Guid.NewGuid(), status: "Todo");

        var result = await service.GetByProjectAsync(projectId, page: 1, pageSize: 20, status: "Todo", priority: null);

        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal("Todo", t.Status));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_FiltersByPriority()
    {
        using var db = CreateDb();
        var service = new TaskService(db);
        var projectId = Guid.NewGuid();
        SeedTask(db, projectId, Guid.NewGuid(), priority: "Low");
        SeedTask(db, projectId, Guid.NewGuid(), priority: "High");
        SeedTask(db, projectId, Guid.NewGuid(), priority: "High");

        var result = await service.GetByProjectAsync(projectId, page: 1, pageSize: 20, status: null, priority: "High");

        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.Equal("High", t.Priority));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_FiltersByStatusAndPriority()
    {
        using var db = CreateDb();
        var service = new TaskService(db);
        var projectId = Guid.NewGuid();
        SeedTask(db, projectId, Guid.NewGuid(), status: "Todo", priority: "High");
        SeedTask(db, projectId, Guid.NewGuid(), status: "Todo", priority: "Low");
        SeedTask(db, projectId, Guid.NewGuid(), status: "Done", priority: "High");

        var result = await service.GetByProjectAsync(projectId, page: 1, pageSize: 20, status: "Todo", priority: "High");

        Assert.Single(result);
        Assert.Equal("Todo", result.First().Status);
        Assert.Equal("High", result.First().Priority);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_DoesNotReturnTasksFromOtherProjects()
    {
        using var db = CreateDb();
        var service = new TaskService(db);
        var projectId = Guid.NewGuid();
        SeedTask(db, projectId, Guid.NewGuid());
        SeedTask(db, Guid.NewGuid(), Guid.NewGuid()); // different project

        var result = await service.GetByProjectAsync(projectId, page: 1, pageSize: 20, status: null, priority: null);

        Assert.Single(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_PaginatesResults()
    {
        using var db = CreateDb();
        var service = new TaskService(db);
        var projectId = Guid.NewGuid();
        for (var i = 0; i < 5; i++)
            SeedTask(db, projectId, Guid.NewGuid());

        var page1 = await service.GetByProjectAsync(projectId, page: 1, pageSize: 2, status: null, priority: null);
        var page2 = await service.GetByProjectAsync(projectId, page: 2, pageSize: 2, status: null, priority: null);
        var page3 = await service.GetByProjectAsync(projectId, page: 3, pageSize: 2, status: null, priority: null);

        Assert.Equal(2, page1.Count());
        Assert.Equal(2, page2.Count());
        Assert.Single(page3);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_ReturnsEmpty_WhenPageBeyondResults()
    {
        using var db = CreateDb();
        var service = new TaskService(db);
        var projectId = Guid.NewGuid();
        SeedTask(db, projectId, Guid.NewGuid());

        var result = await service.GetByProjectAsync(projectId, page: 99, pageSize: 20, status: null, priority: null);

        Assert.Empty(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProjectAsync_IsStable_WhenCreatedAtTimestampsCollide()
    {
        var options = CreateOptions();
        using var db = new AppDbContext(options);
        var projectId = Guid.NewGuid();
        var timestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var idA = new Guid("aaaaaaaa-0000-0000-0000-000000000000");
        var idB = new Guid("bbbbbbbb-0000-0000-0000-000000000000");
        db.Tasks.AddRange(
            new ProjectTask { Id = idA, ProjectId = projectId, Title = "A", Status = "Todo", Priority = "Low", CreatedAt = timestamp },
            new ProjectTask { Id = idB, ProjectId = projectId, Title = "B", Status = "Todo", Priority = "Low", CreatedAt = timestamp });
        db.SaveChanges();
        var service = new TaskService(db);

        var page1 = (await service.GetByProjectAsync(projectId, page: 1, pageSize: 1, status: null, priority: null)).Single();
        var page2 = (await service.GetByProjectAsync(projectId, page: 2, pageSize: 1, status: null, priority: null)).Single();

        Assert.NotEqual(page1.Id, page2.Id);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ReturnsNull_WhenTaskDoesNotExist()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var result = await service.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ReturnsNull_WhenTaskBelongsToDifferentProject()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var taskId = Guid.NewGuid();
        SeedTask(db, projectId: Guid.NewGuid(), taskId: taskId);

        var result = await service.GetByIdAsync(projectId: Guid.NewGuid(), taskId: taskId);

        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_ReturnsTask_WhenFound()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var seeded = SeedTask(db, projectId, taskId);

        var result = await service.GetByIdAsync(projectId, taskId);

        Assert.NotNull(result);
        Assert.Equal(taskId, result.Id);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(seeded.Title, result.Title);
        Assert.Equal(seeded.Status, result.Status);
        Assert.Equal(seeded.Priority, result.Priority);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatusAsync_ReturnsNull_WhenTaskDoesNotExist()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var result = await service.UpdateStatusAsync(Guid.NewGuid(), Guid.NewGuid(), "InProgress");

        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatusAsync_ReturnsNull_WhenTaskBelongsToDifferentProject()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var taskId = Guid.NewGuid();
        SeedTask(db, projectId: Guid.NewGuid(), taskId: taskId);

        var result = await service.UpdateStatusAsync(projectId: Guid.NewGuid(), taskId: taskId, "Done");

        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatusAsync_UpdatesStatus_WhenTaskExists()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        SeedTask(db, projectId, taskId);

        var result = await service.UpdateStatusAsync(projectId, taskId, "InProgress");

        Assert.NotNull(result);
        Assert.Equal("InProgress", result.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatusAsync_PreservesOtherFields()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var seeded = SeedTask(db, projectId, taskId);

        var result = await service.UpdateStatusAsync(projectId, taskId, "Done");

        Assert.NotNull(result);
        Assert.Equal(seeded.Title, result.Title);
        Assert.Equal(seeded.Description, result.Description);
        Assert.Equal(seeded.Priority, result.Priority);
        Assert.Equal(seeded.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateStatusAsync_PersistsStatus_ToDatabase()
    {
        var options = CreateOptions();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedTask(db, projectId, taskId);
            var service = new TaskService(db);
            await service.UpdateStatusAsync(projectId, taskId, "Done");
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Tasks.FindAsync(taskId);
            Assert.NotNull(stored);
            Assert.Equal("Done", stored.Status);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ReturnsNull_WhenTaskDoesNotExist()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var result = await service.UpdateAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new UpdateTaskRequest("New Title", null, "InProgress", "High"));

        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_ReturnsNull_WhenTaskBelongsToDifferentProject()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var taskId = Guid.NewGuid();
        SeedTask(db, projectId: Guid.NewGuid(), taskId: taskId);

        var result = await service.UpdateAsync(
            projectId: Guid.NewGuid(), // different project
            taskId: taskId,
            new UpdateTaskRequest("Updated", null, "Done", "Medium"));

        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_UpdatesAllFields_WhenTaskExists()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        SeedTask(db, projectId, taskId);

        var request = new UpdateTaskRequest(
            Title: "Updated Title",
            Description: "Updated description",
            Status: "InProgress",
            Priority: "High");

        var result = await service.UpdateAsync(projectId, taskId, request);

        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated description", result.Description);
        Assert.Equal("InProgress", result.Status);
        Assert.Equal("High", result.Priority);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_PersistsChanges_ToDatabase()
    {
        // Use a named in-memory database so a second context shares the same store,
        // proving SaveChangesAsync actually wrote through rather than relying on
        // the first context's change tracker to satisfy FindAsync.
        var options = CreateOptions();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedTask(db, projectId, taskId);
            var service = new TaskService(db);
            await service.UpdateAsync(projectId, taskId,
                new UpdateTaskRequest("Persisted", "Desc", "Done", "Low"));
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Tasks.FindAsync(taskId);
            Assert.NotNull(stored);
            Assert.Equal("Persisted", stored.Title);
            Assert.Equal("Done", stored.Status);
        }
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_PreservesProjectIdAndCreatedAt()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var original = SeedTask(db, projectId, taskId);
        // Snapshot the value before calling UpdateAsync so that if the service
        // accidentally mutates CreatedAt on the tracked entity, this assertion
        // still catches it (both references would otherwise reflect the same change).
        var expectedCreatedAt = original.CreatedAt;

        var result = await service.UpdateAsync(projectId, taskId,
            new UpdateTaskRequest("New", null, "Done", "High"));

        Assert.NotNull(result);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(expectedCreatedAt, result.CreatedAt);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_AllowsNullDescription()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        SeedTask(db, projectId, taskId);

        var result = await service.UpdateAsync(projectId, taskId,
            new UpdateTaskRequest("Title", Description: null, "Todo", "Medium"));

        Assert.NotNull(result);
        Assert.Null(result.Description);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ReturnsFalse_WhenTaskDoesNotExist()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var result = await service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ReturnsFalse_WhenTaskBelongsToDifferentProject()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var taskId = Guid.NewGuid();
        SeedTask(db, projectId: Guid.NewGuid(), taskId: taskId);

        var result = await service.DeleteAsync(projectId: Guid.NewGuid(), taskId: taskId);

        Assert.False(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_ReturnsTrue_WhenTaskExists()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        SeedTask(db, projectId, taskId);

        var result = await service.DeleteAsync(projectId, taskId);

        Assert.True(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_RemovesTask_FromDatabase()
    {
        var options = CreateOptions();
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedTask(db, projectId, taskId);
            var service = new TaskService(db);
            await service.DeleteAsync(projectId, taskId);
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Tasks.FindAsync(taskId);
            Assert.Null(stored);
        }
    }
}
