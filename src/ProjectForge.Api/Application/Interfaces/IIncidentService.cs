using ProjectForge.Api.Application.DTOs.Incidents;

namespace ProjectForge.Api.Application.Interfaces;

public interface IIncidentService
{
    Task<IEnumerable<IncidentResponse>> GetByProjectAsync(Guid projectId);
    Task<IncidentResponse> CreateAsync(Guid projectId, CreateIncidentRequest request);

    // TODO: Task<IncidentResponse?> UpdateStatusAsync(Guid incidentId, string status);
    // TODO: Task<IncidentResponse?> UpdateAsync(Guid incidentId, UpdateIncidentRequest request);
    // TODO: Task<bool> DeleteAsync(Guid incidentId);
    // TODO: Add filtering by severity/status.
}
