using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectForge.Api.Application.DTOs.Notes;
using ProjectForge.Api.Application.Interfaces;

namespace ProjectForge.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/notes")]
[Authorize]
[Produces("application/json")]
public class NotesController(INoteService noteService) : ControllerBase
{
    /// <summary>
    /// Returns all notes for a given project.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NoteResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        var notes = await noteService.GetByProjectAsync(projectId);
        return Ok(notes);
    }

    /// <summary>
    /// Returns a single note by ID within a project.
    /// </summary>
    [HttpGet("{noteId:guid}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid projectId, Guid noteId)
    {
        var note = await noteService.GetByIdAsync(projectId, noteId);
        if (note is null)
            return NotFound(new { message = "Note not found." });

        return Ok(note);
    }

    /// <summary>
    /// Creates a new note under a project. Accessible by any authenticated user.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] CreateNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Note content is required." });

        var note = await noteService.CreateAsync(projectId, request);
        return Created($"api/projects/{projectId}/notes/{note.Id}", note);

        // TODO: Add PUT /api/projects/{projectId}/notes/{noteId}
        // TODO: Add DELETE /api/projects/{projectId}/notes/{noteId}
        // TODO: Add GET /api/projects/{projectId}/notes/{noteId} for single note retrieval.
    }
}
