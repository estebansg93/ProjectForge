using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Projects;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Application.Services;

public class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<IEnumerable<ProjectResponse>> GetAllAsync()
    {
        return await db.Projects
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => ToResponse(p))
            .ToListAsync();

        // TODO: Add pagination (page, pageSize parameters).
        // TODO: Add filtering by status.
        // TODO: Include task/note/incident counts in response.
    }

    public async Task<ProjectResponse?> GetByIdAsync(Guid id)
    {
        var project = await db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return project is null ? null : ToResponse(project);

        // TODO: Return a richer ProjectDetailResponse with related resource summaries.
    }

    public async Task<ProjectResponse> CreateAsync(CreateProjectRequest request)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        db.Projects.Add(project);
        await db.SaveChangesAsync();

        return ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
            return null;

        project.Name = request.Name;
        project.Description = request.Description;
        project.Status = request.Status;

        await db.SaveChangesAsync();
        return ToResponse(project);
    }

    public async Task<ProjectResponse?> UpdateStatusAsync(Guid id, string status)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
            return null;

        project.Status = status;
        await db.SaveChangesAsync();
        return ToResponse(project);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == id);
        if (project is null)
            return false;

        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return true;
    }

    private static ProjectResponse ToResponse(Project p) =>
        new(p.Id, p.Name, p.Description, p.Status, p.CreatedAt);
}
