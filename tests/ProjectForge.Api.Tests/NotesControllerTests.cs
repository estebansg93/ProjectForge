using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectForge.Api.Application.DTOs.Notes;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Controllers;

namespace ProjectForge.Api.Tests;

public class NotesControllerTests
{
    private readonly Mock<INoteService> _mockService;
    private readonly NotesController _controller;

    public NotesControllerTests()
    {
        _mockService = new Mock<INoteService>();
        _controller = new NotesController(_mockService.Object);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenContentIsEmpty()
    {
        var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new UpdateNoteRequest(""));

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateNoteRequest>()), Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenContentIsWhitespace()
    {
        var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), new UpdateNoteRequest("   "));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var request = new UpdateNoteRequest("Updated content");

        _mockService
            .Setup(s => s.UpdateAsync(projectId, noteId, request))
            .ReturnsAsync((NoteResponse?)null);

        var result = await _controller.Update(projectId, noteId, request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithUpdatedNote()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var request = new UpdateNoteRequest("Updated content");
        var response = new NoteResponse(noteId, projectId, "Updated content", DateTime.UtcNow);

        _mockService
            .Setup(s => s.UpdateAsync(projectId, noteId, request))
            .ReturnsAsync(response);

        var result = await _controller.Update(projectId, noteId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<NoteResponse>(ok.Value);
        Assert.Equal("Updated content", returned.Content);
    }

    [Fact]
    public async Task Update_CallsService_WithCorrectArguments()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var request = new UpdateNoteRequest("Content");
        var response = new NoteResponse(noteId, projectId, "Content", DateTime.UtcNow);

        _mockService
            .Setup(s => s.UpdateAsync(projectId, noteId, request))
            .ReturnsAsync(response);

        await _controller.Update(projectId, noteId, request);

        _mockService.Verify(s => s.UpdateAsync(projectId, noteId, request), Times.Once);
    }
}
