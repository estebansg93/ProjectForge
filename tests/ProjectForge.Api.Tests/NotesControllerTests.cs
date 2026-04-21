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
    public async Task GetById_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();

        _mockService
            .Setup(s => s.GetByIdAsync(projectId, noteId))
            .ReturnsAsync((NoteResponse?)null);

        var result = await _controller.GetById(projectId, noteId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithNote()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var response = new NoteResponse(noteId, projectId, "Some content", DateTime.UtcNow);

        _mockService
            .Setup(s => s.GetByIdAsync(projectId, noteId))
            .ReturnsAsync(response);

        var result = await _controller.GetById(projectId, noteId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<NoteResponse>(ok.Value);
        Assert.Equal(noteId, returned.Id);
        Assert.Equal("Some content", returned.Content);
    }

    [Fact]
    public async Task GetById_CallsService_WithCorrectArguments()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();
        var response = new NoteResponse(noteId, projectId, "Content", DateTime.UtcNow);

        _mockService
            .Setup(s => s.GetByIdAsync(projectId, noteId))
            .ReturnsAsync(response);

        await _controller.GetById(projectId, noteId);

        _mockService.Verify(s => s.GetByIdAsync(projectId, noteId), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenServiceReturnsFalse()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeleteAsync(projectId, noteId))
            .ReturnsAsync(false);

        var result = await _controller.Delete(projectId, noteId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenServiceReturnsTrue()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeleteAsync(projectId, noteId))
            .ReturnsAsync(true);

        var result = await _controller.Delete(projectId, noteId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_CallsService_WithCorrectArguments()
    {
        var projectId = Guid.NewGuid();
        var noteId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeleteAsync(projectId, noteId))
            .ReturnsAsync(true);

        await _controller.Delete(projectId, noteId);

        _mockService.Verify(s => s.DeleteAsync(projectId, noteId), Times.Once);
    }
}
