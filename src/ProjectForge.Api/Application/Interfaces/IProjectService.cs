using ProjectForge.Api.Application.DTOs.Projects;

namespace ProjectForge.Api.Application.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponse>> GetAllAsync();
    Task<ProjectResponse?> GetByIdAsync(Guid id);
    Task<ProjectResponse> CreateAsync(CreateProjectRequest request);

    // TODO: Task<ProjectResponse?> UpdateAsync(Guid id, UpdateProjectRequest request);
    // TODO: Task<bool> DeleteAsync(Guid id);
    // TODO: Add pagination: Task<PagedResult<ProjectResponse>> GetPagedAsync(int page, int pageSize);
    // TODO: Add filtering by status.
}
