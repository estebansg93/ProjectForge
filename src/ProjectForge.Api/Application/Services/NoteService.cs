using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Notes;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Application.Services;

public class NoteService(AppDbContext db) : INoteService
{
    public async Task<IEnumerable<NoteResponse>> GetByProjectAsync(Guid projectId)
    {
        return await db.Notes
            .AsNoTracking()
            .Where(n => n.ProjectId == projectId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => ToResponse(n))
            .ToListAsync();

        // TODO: Add pagination.
        // TODO: Include author info when user authorship is tracked.
    }

    public async Task<NoteResponse> CreateAsync(Guid projectId, CreateNoteRequest request)
    {
        // TODO: Validate projectId exists.

        var note = new Note
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        db.Notes.Add(note);
        await db.SaveChangesAsync();

        return ToResponse(note);
    }

    public async Task<NoteResponse?> GetByIdAsync(Guid projectId, Guid noteId)
    {
        var note = await db.Notes
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == noteId && n.ProjectId == projectId);

        return note is null ? null : ToResponse(note);
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid noteId)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == noteId && n.ProjectId == projectId);
        if (note is null)
            return false;

        db.Notes.Remove(note);
        await db.SaveChangesAsync();
        return true;
    }

    // TODO: Implement UpdateAsync.

    private static NoteResponse ToResponse(Note n) =>
        new(n.Id, n.ProjectId, n.Content, n.CreatedAt);
}
