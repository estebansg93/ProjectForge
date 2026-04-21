using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.Services;
using ProjectForge.Api.Domain.Entities;
using ProjectForge.Api.Infrastructure.Data;

namespace ProjectForge.Api.Tests;

public class NoteServiceTests
{
    private static DbContextOptions<AppDbContext> CreateOptions() =>
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

    private static AppDbContext CreateDb() => new(CreateOptions());

    private static Note SeedNote(AppDbContext db, Guid projectId, Guid noteId, string content = "Original content")
    {
        var note = new Note
        {
            Id = noteId,
            ProjectId = projectId,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };
        db.Notes.Add(note);
        db.SaveChanges();
        return note;
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNoteDoesNotExist()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var result = await service.DeleteAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNoteBelongsToDifferentProject()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var noteId = Guid.NewGuid();
        SeedNote(db, projectId: Guid.NewGuid(), noteId: noteId);

        var result = await service.DeleteAsync(projectId: Guid.NewGuid(), noteId: noteId);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenNoteExists()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        SeedNote(db, projectId, noteId);

        var result = await service.DeleteAsync(projectId, noteId);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesNote_FromDatabase()
    {
        var options = CreateOptions();
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedNote(db, projectId, noteId);
            var service = new NoteService(db);
            await service.DeleteAsync(projectId, noteId);
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Notes.FindAsync(noteId);
            Assert.Null(stored);
        }
    }
}
