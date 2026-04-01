namespace ProjectForge.Api.Application.DTOs.Notes;

public record CreateNoteRequest(string Content);

public record NoteResponse(
    Guid Id,
    Guid ProjectId,
    string Content,
    DateTime CreatedAt);

// TODO: Add UpdateNoteRequest when PUT /api/projects/{id}/notes/{noteId} is implemented.
// TODO: Add AuthorId/AuthorEmail to NoteResponse when user authorship is tracked.
