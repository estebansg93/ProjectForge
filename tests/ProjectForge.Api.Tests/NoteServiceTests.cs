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
}
