using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectForge.Api.Application.DTOs.Incidents;
using ProjectForge.Api.Application.Interfaces;

namespace ProjectForge.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/incidents")]
[Authorize]
[Produces("application/json")]
public class IncidentsController(IIncidentService incidentService) : ControllerBase
{
    /// <summary>
    /// Returns all incidents for a given project.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<IncidentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var incidents = await incidentService.GetByProjectAsync(projectId);
        return Ok(incidents);
    }

    /// <summary>
    /// Creates a new incident under a project. Accessible by any authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IncidentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateIncidentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Incident title is required." });

        var incident = await incidentService.CreateAsync(projectId, request);
        return Created($"api/projects/{projectId}/incidents/{incident.Id}", incident);

        // TODO: Add PATCH /api/projects/{projectId}/incidents/{incidentId}/status
        // TODO: Add PUT /api/projects/{projectId}/incidents/{incidentId}
        // TODO: Add DELETE /api/projects/{projectId}/incidents/{incidentId}
        // TODO: Add GET /api/projects/{projectId}/incidents/{incidentId} for single retrieval.
    }
}
