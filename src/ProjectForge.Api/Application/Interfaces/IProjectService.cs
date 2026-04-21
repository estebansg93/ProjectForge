using ProjectForge.Api.Application.DTOs.Projects;

namespace ProjectForge.Api.Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectSummaryResponse>> GetAllAsync(int page, int pageSize, string? status);
    Task<ProjectDetailResponse?> GetByIdAsync(Guid id);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);
    Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<ProjectResponse?> UpdateStatusAsync(Guid id, string status);
    Task<bool> DeleteAsync(Guid id);

    // TODO: Add pagination: Task<PagedResult<ProjectResponse>> GetPagedAsync(int page, int pageSize);
    // TODO: Add filtering by status.
}
