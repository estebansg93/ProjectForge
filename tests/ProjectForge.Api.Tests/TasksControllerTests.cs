using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectForge.Api.Application.DTOs.Tasks;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Controllers;

namespace ProjectForge.Api.Tests;

public class TasksControllerTests
{
    private readonly Mock<ITaskService> _mockService;
    private readonly TasksController _controller;

    public TasksControllerTests()
    {
        _mockService = new Mock<ITaskService>();
        _controller = new TasksController(_mockService.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetById_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _mockService
            .Setup(s => s.GetByIdAsync(projectId, taskId))
            .ReturnsAsync((TaskResponse?)null);

        var result = await _controller.GetById(projectId, taskId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetById_ReturnsOk_WithTask()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var response = new TaskResponse(taskId, projectId, "My Task", null, "Todo", "Low", DateTime.UtcNow);

        _mockService
            .Setup(s => s.GetByIdAsync(projectId, taskId))
            .ReturnsAsync(response);

        var result = await _controller.GetById(projectId, taskId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TaskResponse>(ok.Value);
        Assert.Equal(taskId, returned.Id);
        Assert.Equal("My Task", returned.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetById_CallsService_WithCorrectArguments()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var response = new TaskResponse(taskId, projectId, "Title", null, "Todo", "Low", DateTime.UtcNow);

        _mockService
            .Setup(s => s.GetByIdAsync(projectId, taskId))
            .ReturnsAsync(response);

        await _controller.GetById(projectId, taskId);

        _mockService.Verify(s => s.GetByIdAsync(projectId, taskId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_ReturnsBadRequest_WhenTitleIsEmpty()
    {
        var request = new UpdateTaskRequest(
            Title: "",
            Description: null,
            Status: "Todo",
            Priority: "Medium");

        var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), request);

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateTaskRequest>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_ReturnsBadRequest_WhenTitleIsWhitespace()
    {
        var request = new UpdateTaskRequest(
            Title: "   ",
            Description: null,
            Status: "Todo",
            Priority: "Medium");

        var result = await _controller.Update(Guid.NewGuid(), Guid.NewGuid(), request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest("Title", null, "Todo", "Medium");

        _mockService
            .Setup(s => s.UpdateAsync(projectId, taskId, request))
            .ReturnsAsync((TaskResponse?)null);

        var result = await _controller.Update(projectId, taskId, request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_ReturnsOk_WithUpdatedTask()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest("Updated Title", "Some desc", "InProgress", "High");

        var response = new TaskResponse(
            taskId, projectId, "Updated Title", "Some desc", "InProgress", "High", DateTime.UtcNow);

        _mockService
            .Setup(s => s.UpdateAsync(projectId, taskId, request))
            .ReturnsAsync(response);

        var result = await _controller.Update(projectId, taskId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TaskResponse>(ok.Value);
        Assert.Equal("Updated Title", returned.Title);
        Assert.Equal("InProgress", returned.Status);
        Assert.Equal("High", returned.Priority);
    }

    [Fact]
    public async System.Threading.Tasks.Task Update_CallsService_WithCorrectArguments()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskRequest("Title", null, "Done", "Low");

        var response = new TaskResponse(
            taskId, projectId, "Title", null, "Done", "Low", DateTime.UtcNow);

        _mockService
            .Setup(s => s.UpdateAsync(projectId, taskId, request))
            .ReturnsAsync(response);

        await _controller.Update(projectId, taskId, request);

        _mockService.Verify(s => s.UpdateAsync(projectId, taskId, request), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_ReturnsNotFound_WhenServiceReturnsFalse()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeleteAsync(projectId, taskId))
            .ReturnsAsync(false);

        var result = await _controller.Delete(projectId, taskId);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_ReturnsNoContent_WhenServiceReturnsTrue()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeleteAsync(projectId, taskId))
            .ReturnsAsync(true);

        var result = await _controller.Delete(projectId, taskId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task Delete_CallsService_WithCorrectArguments()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _mockService
            .Setup(s => s.DeleteAsync(projectId, taskId))
            .ReturnsAsync(true);

        await _controller.Delete(projectId, taskId);

        _mockService.Verify(s => s.DeleteAsync(projectId, taskId), Times.Once);
    }
}
