using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Incidents;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Application.Services;

public class IncidentService(AppDbContext db) : IIncidentService
{
    public async Task<IEnumerable<IncidentResponse>> GetByProjectAsync(Guid projectId)
    {
        return await db.Incidents
            .AsNoTracking()
            .Where(i => i.ProjectId == projectId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => ToResponse(i))
            .ToListAsync();

        // TODO: Add filtering by severity/status.
        // TODO: Add pagination.
    }

    public async Task<IncidentResponse> CreateAsync(Guid projectId, CreateIncidentRequest request)
    {
        // TODO: Validate projectId exists.

        var incident = new Incident
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = request.Title,
            Description = request.Description,
            Severity = request.Severity,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        db.Incidents.Add(incident);
        await db.SaveChangesAsync();

        return ToResponse(incident);
    }

    // TODO: Implement UpdateStatusAsync (e.g., PATCH /incidents/{id}/status).
    // TODO: Implement full UpdateAsync.
    // TODO: Implement DeleteAsync.

    private static IncidentResponse ToResponse(Incident i) =>
        new(i.Id, i.ProjectId, i.Title, i.Description, i.Severity, i.Status, i.CreatedAt);
}
