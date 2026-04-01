using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectForge.Api.Application.DTOs.Tasks;
using ProjectForge.Api.Application.Interfaces;

namespace ProjectForge.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
[Authorize]
[Produces("application/json")]
public class TasksController(ITaskService taskService) : ControllerBase
{
    /// <summary>
    /// Returns all tasks for a given project.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var tasks = await taskService.GetByProjectAsync(projectId);
        return Ok(tasks);
    }

    /// <summary>
    /// Creates a new task under a project. Accessible by any authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Task title is required." });

        var task = await taskService.CreateAsync(projectId, request);
        return Created($"api/projects/{projectId}/tasks/{task.Id}", task);

        // TODO: Add PUT /api/projects/{projectId}/tasks/{taskId}
        // TODO: Add DELETE /api/projects/{projectId}/tasks/{taskId}
        // TODO: Add PATCH /api/projects/{projectId}/tasks/{taskId}/status for status transitions.
        // TODO: Add GET /api/projects/{projectId}/tasks/{taskId} for single task retrieval.
    }
}
