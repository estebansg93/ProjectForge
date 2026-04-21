namespace ProjectForge.Api.Application.DTOs.Notes;

public record CreateNoteRequest(string Content);

public record NoteResponse(
    Guid Id,
    Guid ProjectId,
    string Content,
    DateTime CreatedAt);

public record UpdateNoteRequest(string Content);
// TODO: Add AuthorId/AuthorEmail to NoteResponse when user authorship is tracked.
