using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Tasks;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Application.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<IEnumerable<TaskResponse>> GetByProjectAsync(Guid projectId)
    {
        return await db.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => ToResponse(t))
            .ToListAsync();

        // TODO: Add filtering by status and priority.
        // TODO: Add pagination.
        // TODO: Include assignee info when user assignment is implemented.
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

    // TODO: Implement UpdateAsync including status transitions.
    // TODO: Implement DeleteAsync.

    private static TaskResponse ToResponse(ProjectTask t) =>
        new(t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.Priority, t.CreatedAt);
}
