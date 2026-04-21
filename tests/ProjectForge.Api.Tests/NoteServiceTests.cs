using Microsoft.EntityFrameworkCore;
using ProjectForge.Api.Application.DTOs.Notes;
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
    public async Task GetByIdAsync_ReturnsNull_WhenNoteDoesNotExist()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var result = await service.GetByIdAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNoteBelongsToDifferentProject()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var noteId = Guid.NewGuid();
        SeedNote(db, projectId: Guid.NewGuid(), noteId: noteId);

        var result = await service.GetByIdAsync(projectId: Guid.NewGuid(), noteId: noteId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNote_WhenFound()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var seeded = SeedNote(db, projectId, noteId);

        var result = await service.GetByIdAsync(projectId, noteId);

        Assert.NotNull(result);
        Assert.Equal(noteId, result.Id);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(seeded.Content, result.Content);
        Assert.Equal(seeded.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNoteDoesNotExist()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var result = await service.UpdateAsync(Guid.NewGuid(), Guid.NewGuid(), new UpdateNoteRequest("New content"));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNoteBelongsToDifferentProject()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var noteId = Guid.NewGuid();
        SeedNote(db, projectId: Guid.NewGuid(), noteId: noteId);

        var result = await service.UpdateAsync(projectId: Guid.NewGuid(), noteId: noteId, new UpdateNoteRequest("Updated"));

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesContent_WhenNoteExists()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        SeedNote(db, projectId, noteId);

        var result = await service.UpdateAsync(projectId, noteId, new UpdateNoteRequest("Updated content"));

        Assert.NotNull(result);
        Assert.Equal("Updated content", result.Content);
    }

    [Fact]
    public async Task UpdateAsync_PreservesProjectIdAndCreatedAt()
    {
        using var db = CreateDb();
        var service = new NoteService(db);

        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var seeded = SeedNote(db, projectId, noteId);
        var expectedCreatedAt = seeded.CreatedAt;

        var result = await service.UpdateAsync(projectId, noteId, new UpdateNoteRequest("Updated"));

        Assert.NotNull(result);
        Assert.Equal(projectId, result.ProjectId);
        Assert.Equal(expectedCreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_PersistsChanges_ToDatabase()
    {
        var options = CreateOptions();
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();

        using (var db = new AppDbContext(options))
        {
            SeedNote(db, projectId, noteId);
            var service = new NoteService(db);
            await service.UpdateAsync(projectId, noteId, new UpdateNoteRequest("Persisted content"));
        }

        using (var db = new AppDbContext(options))
        {
            var stored = await db.Notes.FindAsync(noteId);
            Assert.NotNull(stored);
            Assert.Equal("Persisted content", stored.Content);
        }
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
