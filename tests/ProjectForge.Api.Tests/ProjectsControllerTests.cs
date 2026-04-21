using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectForge.Api.Application.DTOs.Projects;
using ProjectForge.Api.Application.Interfaces;
using ProjectForge.Api.Controllers;

namespace ProjectForge.Api.Tests;

public class ProjectsControllerTests
{
    private readonly Mock<IProjectService> _mockService;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _mockService = new Mock<IProjectService>();
        _controller = new ProjectsController(_mockService.Object);
    }

    // --- GetAll ---

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenPageIsZero()
    {
        var result = await _controller.GetAll(page: 0);

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()), Times.Never);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenPageSizeIsZero()
    {
        var result = await _controller.GetAll(pageSize: 0);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenPageSizeExceedsMax()
    {
        var result = await _controller.GetAll(pageSize: 101);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenPageOffsetOverflows()
    {
        var result = await _controller.GetAll(page: int.MaxValue, pageSize: 100);

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()), Times.Never);
    }

    [Fact]
    public async Task GetAll_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        var result = await _controller.GetAll(status: "InvalidStatus");

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()), Times.Never);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WithDefaults()
    {
        _mockService
            .Setup(s => s.GetAllAsync(1, 20, null))
            .ReturnsAsync([]);

        var result = await _controller.GetAll();

        Assert.IsType<OkObjectResult>(result);
        _mockService.Verify(s => s.GetAllAsync(1, 20, null), Times.Once);
    }

    [Fact]
    public async Task GetAll_PassesParametersToService()
    {
        _mockService
            .Setup(s => s.GetAllAsync(2, 10, "Active"))
            .ReturnsAsync([]);

        await _controller.GetAll(page: 2, pageSize: 10, status: "Active");

        _mockService.Verify(s => s.GetAllAsync(2, 10, "Active"), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenStatusIsNull()
    {
        _mockService
            .Setup(s => s.GetAllAsync(1, 20, null))
            .ReturnsAsync([]);

        var result = await _controller.GetAll(status: null);

        Assert.IsType<OkObjectResult>(result);
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync((ProjectDetailResponse?)null);

        var result = await _controller.GetById(id);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithDetailResponse()
    {
        var id = Guid.NewGuid();
        var response = new ProjectDetailResponse(id, "Project X", null, "Active", DateTime.UtcNow, 3, 1, 2);

        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(response);

        var result = await _controller.GetById(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<ProjectDetailResponse>(ok.Value);
        Assert.Equal(id, returned.Id);
        Assert.Equal(3, returned.TaskCount);
        Assert.Equal(1, returned.NoteCount);
        Assert.Equal(2, returned.IncidentCount);
    }

    [Fact]
    public async Task GetById_CallsService_WithCorrectId()
    {
        var id = Guid.NewGuid();
        var response = new ProjectDetailResponse(id, "P", null, "Active", DateTime.UtcNow, 0, 0, 0);

        _mockService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(response);

        await _controller.GetById(id);

        _mockService.Verify(s => s.GetByIdAsync(id), Times.Once);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenNameIsEmpty()
    {
        var result = await _controller.Update(Guid.NewGuid(), new UpdateProjectRequest("", null, "Active"));

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProjectRequest>()), Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenNameIsWhitespace()
    {
        var result = await _controller.Update(Guid.NewGuid(), new UpdateProjectRequest("   ", null, "Active"));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        var result = await _controller.Update(Guid.NewGuid(), new UpdateProjectRequest("Name", null, "InvalidStatus"));

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<UpdateProjectRequest>()), Times.Never);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var id = Guid.NewGuid();
        var request = new UpdateProjectRequest("Name", null, "Active");

        _mockService.Setup(s => s.UpdateAsync(id, request)).ReturnsAsync((ProjectResponse?)null);

        var result = await _controller.Update(id, request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WithUpdatedProject()
    {
        var id = Guid.NewGuid();
        var request = new UpdateProjectRequest("Updated", "Desc", "Completed");
        var response = new ProjectResponse(id, "Updated", "Desc", "Completed", DateTime.UtcNow);

        _mockService.Setup(s => s.UpdateAsync(id, request)).ReturnsAsync(response);

        var result = await _controller.Update(id, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<ProjectResponse>(ok.Value);
        Assert.Equal("Updated", returned.Name);
        Assert.Equal("Completed", returned.Status);
    }

    [Fact]
    public async Task Update_CallsService_WithCorrectArguments()
    {
        var id = Guid.NewGuid();
        var request = new UpdateProjectRequest("Name", null, "Active");
        var response = new ProjectResponse(id, "Name", null, "Active", DateTime.UtcNow);

        _mockService.Setup(s => s.UpdateAsync(id, request)).ReturnsAsync(response);

        await _controller.Update(id, request);

        _mockService.Verify(s => s.UpdateAsync(id, request), Times.Once);
    }

    // --- PatchStatus ---

    [Fact]
    public async Task PatchStatus_ReturnsBadRequest_WhenStatusIsEmpty()
    {
        var result = await _controller.PatchStatus(Guid.NewGuid(), new PatchProjectStatusRequest(""));

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PatchStatus_ReturnsBadRequest_WhenStatusIsWhitespace()
    {
        var result = await _controller.PatchStatus(Guid.NewGuid(), new PatchProjectStatusRequest("   "));

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task PatchStatus_ReturnsBadRequest_WhenStatusIsInvalid()
    {
        var result = await _controller.PatchStatus(Guid.NewGuid(), new PatchProjectStatusRequest("NotAStatus"));

        Assert.IsType<BadRequestObjectResult>(result);
        _mockService.Verify(s => s.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task PatchStatus_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.UpdateStatusAsync(id, "Archived")).ReturnsAsync((ProjectResponse?)null);

        var result = await _controller.PatchStatus(id, new PatchProjectStatusRequest("Archived"));

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task PatchStatus_ReturnsOk_WithUpdatedProject()
    {
        var id = Guid.NewGuid();
        var response = new ProjectResponse(id, "Project", null, "Archived", DateTime.UtcNow);

        _mockService.Setup(s => s.UpdateStatusAsync(id, "Archived")).ReturnsAsync(response);

        var result = await _controller.PatchStatus(id, new PatchProjectStatusRequest("Archived"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<ProjectResponse>(ok.Value);
        Assert.Equal("Archived", returned.Status);
    }

    [Fact]
    public async Task PatchStatus_CallsService_WithCorrectArguments()
    {
        var id = Guid.NewGuid();
        var response = new ProjectResponse(id, "Project", null, "Completed", DateTime.UtcNow);

        _mockService.Setup(s => s.UpdateStatusAsync(id, "Completed")).ReturnsAsync(response);

        await _controller.PatchStatus(id, new PatchProjectStatusRequest("Completed"));

        _mockService.Verify(s => s.UpdateStatusAsync(id, "Completed"), Times.Once);
    }

    // --- Delete ---

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenServiceReturnsFalse()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(false);

        var result = await _controller.Delete(id);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenServiceReturnsTrue()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_CallsService_WithCorrectArguments()
    {
        var id = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        await _controller.Delete(id);

        _mockService.Verify(s => s.DeleteAsync(id), Times.Once);
    }
}
