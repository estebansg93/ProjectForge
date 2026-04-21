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
    public async System.Threading.Tasks.Task GetByProject_ReturnsBadRequest_WhenPageIsZero()
    {
        var result = await _controller.GetByProject(Guid.NewGuid(), page: 0);

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.GetByProjectAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProject_ReturnsBadRequest_WhenPageSizeIsZero()
    {
        var result = await _controller.GetByProject(Guid.NewGuid(), pageSize: 0);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProject_ReturnsBadRequest_WhenPageSizeExceedsMax()
    {
        var result = await _controller.GetByProject(Guid.NewGuid(), pageSize: 101);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProject_ReturnsBadRequest_WhenPageOffsetOverflows()
    {
        var result = await _controller.GetByProject(Guid.NewGuid(), page: int.MaxValue, pageSize: 100);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProject_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        var result = await _controller.GetByProject(Guid.NewGuid(), status: "Invalid");

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.GetByProjectAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProject_ReturnsBadRequest_WhenPriorityIsInvalid()
    {
        var result = await _controller.GetByProject(Guid.NewGuid(), priority: "Urgent");

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProject_ReturnsOk_WithDefaults()
    {
        var projectId = Guid.NewGuid();
        _mockService
            .Setup(s => s.GetByProjectAsync(projectId, 1, 20, null, null))
            .ReturnsAsync([]);

        var result = await _controller.GetByProject(projectId);

        Assert.IsType<OkObjectResult>(result);
        _mockService.Verify(s => s.GetByProjectAsync(projectId, 1, 20, null, null), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByProject_PassesAllParametersToService()
    {
        var projectId = Guid.NewGuid();
        _mockService
            .Setup(s => s.GetByProjectAsync(projectId, 2, 10, "Todo", "High"))
            .ReturnsAsync([]);

        await _controller.GetByProject(projectId, page: 2, pageSize: 10, status: "Todo", priority: "High");

        _mockService.Verify(s => s.GetByProjectAsync(projectId, 2, 10, "Todo", "High"), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task PatchStatus_ReturnsBadRequest_WhenStatusIsEmpty()
    {
        var result = await _controller.PatchStatus(
            Guid.NewGuid(), Guid.NewGuid(), new PatchTaskStatusRequest(""));

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async System.Threading.Tasks.Task PatchStatus_ReturnsBadRequest_WhenStatusIsWhitespace()
    {
        var result = await _controller.PatchStatus(
            Guid.NewGuid(), Guid.NewGuid(), new PatchTaskStatusRequest("   "));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task PatchStatus_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();

        _mockService
            .Setup(s => s.UpdateStatusAsync(projectId, taskId, "Done"))
            .ReturnsAsync((TaskResponse?)null);

        var result = await _controller.PatchStatus(projectId, taskId, new PatchTaskStatusRequest("Done"));

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task PatchStatus_ReturnsOk_WithUpdatedTask()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var response = new TaskResponse(taskId, projectId, "Title", null, "InProgress", "Low", DateTime.UtcNow);

        _mockService
            .Setup(s => s.UpdateStatusAsync(projectId, taskId, "InProgress"))
            .ReturnsAsync(response);

        var result = await _controller.PatchStatus(projectId, taskId, new PatchTaskStatusRequest("InProgress"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TaskResponse>(ok.Value);
        Assert.Equal("InProgress", returned.Status);
    }

    [Fact]
    public async System.Threading.Tasks.Task PatchStatus_CallsService_WithCorrectArguments()
    {
        var projectId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var response = new TaskResponse(taskId, projectId, "Title", null, "Done", "Low", DateTime.UtcNow);

        _mockService
            .Setup(s => s.UpdateStatusAsync(projectId, taskId, "Done"))
            .ReturnsAsync(response);

        await _controller.PatchStatus(projectId, taskId, new PatchTaskStatusRequest("Done"));

        _mockService.Verify(s => s.UpdateStatusAsync(projectId, taskId, "Done"), Times.Once);
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
