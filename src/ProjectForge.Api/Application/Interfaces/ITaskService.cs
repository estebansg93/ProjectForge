using ProjectForge.Api.Application.DTOs.Tasks;

namespace ProjectForge.Api.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetByProjectAsync(Guid projectId);
    Task<TaskResponse> CreateAsync(Guid projectId, CreateTaskRequest request);

    Task<TaskResponse?> UpdateAsync(Guid projectId, Guid taskId, UpdateTaskRequest request);

    // TODO: Task<bool> DeleteAsync(Guid taskId);
    // TODO: Task<TaskResponse?> UpdateStatusAsync(Guid taskId, string status); — useful partial endpoint
    // TODO: Add filtering by status/priority.
}
