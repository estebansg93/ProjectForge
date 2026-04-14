using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Tasks;
using ProjectForge.Api.Application.Services;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Tests;

public class TaskServiceTests
{
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static ProjectTask SeedTask(AppDbContext db, Guid projectId, Guid taskId)
    {
        var task = new ProjectTask
        {
            Id = taskId,
            ProjectId = projectId,
            Title = "Original Title",
            Description = "Original description",
            Status = "Todo",
            Priority = "Low",
            CreatedAt = DateTime.UtcNow
        };
        db.Tasks.Add(task);
        db.SaveChanges();
        return task;
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
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        SeedTask(db, projectId, taskId);

        await service.UpdateAsync(projectId, taskId,
            new UpdateTaskRequest("Persisted", "Desc", "Done", "Low"));

        var stored = await db.Tasks.FindAsync(taskId);
        Assert.NotNull(stored);
        Assert.Equal("Persisted", stored.Title);
        Assert.Equal("Done", stored.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_PreservesProjectIdAndCreatedAt()
    {
        using var db = CreateDb();
        var service = new TaskService(db);

        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var original = SeedTask(db, projectId, taskId);

        var result = await service.UpdateAsync(projectId, taskId,
            new UpdateTaskRequest("New", null, "Done", "High"));

        Assert.NotNull(result);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(original.CreatedAt, result.CreatedAt);
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
}
