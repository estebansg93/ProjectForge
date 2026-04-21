using ProjectForge.Api.Application.DTOs.Notes;

namespace ProjectForge.Api.Application.Interfaces;

public interface INoteService
{
    Task<IEnumerable<NoteResponse>> GetByProjectAsync(Guid projectId);
    Task<NoteResponse> CreateAsync(Guid projectId, CreateNoteRequest request);
    Task<NoteResponse?> GetByIdAsync(Guid projectId, Guid noteId);
    Task<NoteResponse?> UpdateAsync(Guid projectId, Guid noteId, UpdateNoteRequest request);
    Task<bool> DeleteAsync(Guid projectId, Guid noteId);
}
