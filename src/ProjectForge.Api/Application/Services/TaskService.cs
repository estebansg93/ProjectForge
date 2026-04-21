using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Tasks;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Application.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<IEnumerable<TaskResponse>> GetByProjectAsync(Guid projectId, int page, int pageSize, string? status, string? priority)
    {
        return await db.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .Where(t => status == null || t.Status == status)
            .Where(t => priority == null || t.Priority == priority)
            .OrderByDescending(t => t.CreatedAt)
            .ThenByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => ToResponse(t))
            .ToListAsync();

        // TODO: Include assignee info when user assignment is implemented.
    }

    public async Task<TaskResponse?> GetByIdAsync(Guid projectId, Guid taskId)
    {
        var task = await db.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);

        return task is null ? null : ToResponse(task);
    }

    public async Task<TaskResponse> CreateAsync(Guid projectId, CreateTaskRequest request)
    {
        // TODO: Validate that projectId exists — return 404 if project not found.
        //       This is intentionally left for the controller or a domain validation layer.

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Status = "Todo",
            Priority = request.Priority,
            CreatedAt = DateTime.UtcNow
        };

        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task<TaskResponse?> UpdateAsync(Guid projectId, Guid taskId, UpdateTaskRequest request)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
        if (task is null)
            return null;

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.Priority = request.Priority;

        await db.SaveChangesAsync();
        return ToResponse(task);
    }

    public async Task<TaskResponse?> UpdateStatusAsync(Guid projectId, Guid taskId, string status)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
        if (task is null)
            return null;

        task.Status = status;
        await db.SaveChangesAsync();
        return ToResponse(task);
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid taskId)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ProjectId == projectId);
        if (task is null)
            return false;

        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return true;
    }

    private static TaskResponse ToResponse(ProjectTask t) =>
        new(t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.Priority, t.CreatedAt);
}
