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

    }

    /// <summary>
    /// Updates the content of an existing note.
    /// </summary>
    [HttpPut("{noteId:guid}")]
    [ProducesResponseType(typeof(NoteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid projectId, Guid noteId, [FromBody] UpdateNoteRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
            return BadRequest(new { message = "Note content is required." });

        var note = await noteService.UpdateAsync(projectId, noteId, request);
        if (note is null)
            return NotFound(new { message = "Note not found." });

        return Ok(note);
    }

    /// <summary>
    /// Deletes a note by ID within a project.
    /// </summary>
    [HttpDelete("{noteId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid projectId, Guid noteId)
    {
        var deleted = await noteService.DeleteAsync(projectId, noteId);
        if (!deleted)
            return NotFound(new { message = "Note not found." });

        return NoContent();
    }
}
