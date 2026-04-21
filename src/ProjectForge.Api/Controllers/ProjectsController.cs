using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectForge.Api.Application.DTOs.Projects;
using ProjectForge.Api.Application.Interfaces;

namespace ProjectForge.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
[Produces("application/json")]
public class ProjectsController(IProjectService projectService) : ControllerBase
{
    private static readonly HashSet<string> ValidStatuses = ["Active", "Archived", "Completed"];

    /// <summary>
    /// Returns a paginated, optionally filtered list of projects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null)
    {
        if (page < 1)
            return BadRequest(new { message = "Page must be greater than or equal to 1." });

        if (pageSize < 1 || pageSize > 100)
            return BadRequest(new { message = "Page size must be between 1 and 100." });

        if ((long)(page - 1) * pageSize > int.MaxValue)
            return BadRequest(new { message = "Page value is too large." });

        if (status is not null && !ValidStatuses.Contains(status))
            return BadRequest(new { message = $"Invalid status. Allowed values: {string.Join(", ", ValidStatuses)}." });

        var projects = await projectService.GetAllAsync(page, pageSize, status);
        return Ok(projects);
    }

    /// <summary>
    /// Returns a single project by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var project = await projectService.GetByIdAsync(id);

        if (project is null)
            return NotFound(new { message = $"Project {id} not found." });

        return Ok(project);
    }

    /// <summary>
    /// Creates a new project. Restricted to Admin role.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Project name is required." });

        var project = await projectService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    /// <summary>
    /// Updates a project's name, description, and status. Restricted to Admin role.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(new { message = "Project name is required." });

        if (!ValidStatuses.Contains(request.Status))
            return BadRequest(new { message = $"Invalid status. Allowed values: {string.Join(", ", ValidStatuses)}." });

        var project = await projectService.UpdateAsync(id, request);
        if (project is null)
            return NotFound(new { message = $"Project {id} not found." });

        return Ok(project);
    }

    /// <summary>
    /// Updates a project's status. Restricted to Admin role.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchStatus(Guid id, [FromBody] PatchProjectStatusRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
            return BadRequest(new { message = "Status is required." });

        if (!ValidStatuses.Contains(request.Status))
            return BadRequest(new { message = $"Invalid status. Allowed values: {string.Join(", ", ValidStatuses)}." });

        var project = await projectService.UpdateStatusAsync(id, request.Status);
        if (project is null)
            return NotFound(new { message = $"Project {id} not found." });

        return Ok(project);
    }

    /// <summary>
    /// Deletes a project and all its related data. Restricted to Admin role.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await projectService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Project {id} not found." });

        return NoContent();
    }
}
