using ProjectForge.Api.Application.DTOs.Tasks;

namespace ProjectForge.Api.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponse>> GetByProjectAsync(Guid projectId, int page, int pageSize, string? status, string? priority);
    Task<TaskResponse?> GetByIdAsync(Guid projectId, Guid taskId);
    Task<TaskResponse?> CreateAsync(Guid projectId, CreateTaskRequest request);
    Task<TaskResponse?> UpdateAsync(Guid projectId, Guid taskId, UpdateTaskRequest request);
    Task<bool> DeleteAsync(Guid projectId, Guid taskId);
    Task<TaskResponse?> UpdateStatusAsync(Guid projectId, Guid taskId, string status);
}
