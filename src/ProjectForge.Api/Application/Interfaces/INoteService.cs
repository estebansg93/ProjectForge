using ProjectForge.Api.Application.DTOs.Notes;

namespace ProjectForge.Api.Application.Interfaces;

public interface INoteService
{
    Task<IEnumerable<NoteResponse>> GetByProjectAsync(Guid projectId);
    Task<NoteResponse> CreateAsync(Guid projectId, CreateNoteRequest request);
    Task<NoteResponse?> UpdateAsync(Guid projectId, Guid noteId, UpdateNoteRequest request);

    // TODO: Task<bool> DeleteAsync(Guid noteId);
}
