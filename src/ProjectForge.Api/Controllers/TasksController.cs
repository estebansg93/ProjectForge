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
    private static readonly HashSet<string> ValidStatuses = ["Todo", "InProgress", "Done"];
    private static readonly HashSet<string> ValidPriorities = ["Low", "Medium", "High"];

    /// <summary>
    /// Returns a paginated, optionally filtered list of tasks for a given project.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByProject(
        Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? priority = null)
    {
        if (page < 1)
            return BadRequest(new { message = "Page must be greater than or equal to 1." });

        if (pageSize < 1 || pageSize > 100)
            return BadRequest(new { message = "Page size must be between 1 and 100." });

        if ((long)(page - 1) * pageSize > int.MaxValue)
            return BadRequest(new { message = "Page value is too large." });

        if (status is not null && !ValidStatuses.Contains(status))
            return BadRequest(new { message = $"Invalid status. Allowed values: {string.Join(", ", ValidStatuses)}." });

        if (priority is not null && !ValidPriorities.Contains(priority))
            return BadRequest(new { message = $"Invalid priority. Allowed values: {string.Join(", ", ValidPriorities)}." });

        var tasks = await taskService.GetByProjectAsync(projectId, page, pageSize, status, priority);
        return Ok(tasks);
    }

    /// <summary>
    /// Returns a single task by ID within a project.
    /// </summary>
    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid projectId, Guid taskId)
    {
        var task = await taskService.GetByIdAsync(projectId, taskId);
        if (task is null)
            return NotFound(new { message = "Task not found." });

        return Ok(task);
    }

    /// <summary>
    /// Updates the status of a task.
    /// </summary>
    [HttpPatch("{taskId:guid}/status")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchStatus(Guid projectId, Guid taskId, [FromBody] PatchTaskStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
            return BadRequest(new { message = "Status is required." });

        var task = await taskService.UpdateStatusAsync(projectId, taskId, request.Status);
        if (task is null)
            return NotFound(new { message = "Task not found." });

        return Ok(task);
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
        if (task is null)
            return NotFound(new { message = $"Project {projectId} not found." });

        return Created($"api/projects/{projectId}/tasks/{task.Id}", task);
    }

    /// <summary>
    /// Deletes a task by ID within a project.
    /// </summary>
    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid projectId, Guid taskId)
    {
        var deleted = await taskService.DeleteAsync(projectId, taskId);
        if (!deleted)
            return NotFound(new { message = "Task not found." });

        return NoContent();
    }

    /// <summary>
    /// Updates an existing task. Replaces all editable fields (title, description, status, priority).
    /// </summary>
    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid projectId, Guid taskId, [FromBody] UpdateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Task title is required." });

        var task = await taskService.UpdateAsync(projectId, taskId, request);
        if (task is null)
            return NotFound(new { message = "Task not found." });

        return Ok(task);
    }
}
