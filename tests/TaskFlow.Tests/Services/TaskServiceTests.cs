using FluentAssertions;
using Moq;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services.Implementations;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IPendingLogRepository> _pendingRepoMock;
    private readonly TaskService _sut;

    public TaskServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _pendingRepoMock = new Mock<IPendingLogRepository>();
        _sut = new TaskService(_taskRepoMock.Object, _pendingRepoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTask_WithOpenStatus()
    {
        var dto = new CreateTaskDto { Title = "New Task", Description = "Desc", DueDate = DateTime.UtcNow.AddDays(7) };
        TaskItem? capturedTask = null;
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskItem>())).Callback<TaskItem>(t => capturedTask = t).Returns(Task.CompletedTask);
        var result = await _sut.CreateAsync(1, dto);
        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Status.Should().Be(0);
        capturedTask!.OwnerId.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTask_WhenExists()
    {
        var task = new TaskItem { Id = 1, OwnerId = 1, Title = "Old", Status = Domain.Enums.TaskStatus.Open, CreatedAt = DateTime.UtcNow };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        var dto = new UpdateTaskDto { Title = "New", Description = "New Desc" };
        var result = await _sut.UpdateAsync(1, dto);
        result.Should().NotBeNull();
        result!.Title.Should().Be("New");
    }

    [Fact]
    public async Task MarkAsPendingAsync_ShouldCreatePendingLog_AndSetStatusPending()
    {
        var task = new TaskItem { Id = 1, OwnerId = 1, Title = "T", Status = Domain.Enums.TaskStatus.Open, PendingLogs = new List<PendingLog>() };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        var dto = new CreatePendingLogDto { TaskId = 1, Reason = "Missing docs" };
        var result = await _sut.MarkAsPendingAsync(1, dto);
        result.Should().BeTrue();
        task.Status.Should().Be(Domain.Enums.TaskStatus.Pending);
    }

    [Fact]
    public async Task MarkAsCompletedAsync_ShouldSucceed_WhenNoActivePending()
    {
        var task = new TaskItem { Id = 1, OwnerId = 1, Title = "T", Status = Domain.Enums.TaskStatus.Open };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.HasActivePendingLogAsync(1)).ReturnsAsync(false);
        var result = await _sut.MarkAsCompletedAsync(1);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task MarkAsCompletedAsync_ShouldThrow_WhenHasActivePending()
    {
        var task = new TaskItem { Id = 1, OwnerId = 1, Title = "T", Status = Domain.Enums.TaskStatus.Open };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        _taskRepoMock.Setup(r => r.HasActivePendingLogAsync(1)).ReturnsAsync(true);
        await _sut.Invoking(s => s.MarkAsCompletedAsync(1)).Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        await _sut.DeleteAsync(1);
        _taskRepoMock.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task ResolvePendingAsync_ShouldResolveAndReopen()
    {
        var log = new PendingLog { Id = 1, TaskId = 1, OwnerId = 1, Reason = "X", CreatedAt = DateTime.UtcNow };
        var task = new TaskItem { Id = 1, OwnerId = 1, Title = "T", Status = Domain.Enums.TaskStatus.Pending, PendingLogs = new List<PendingLog> { log } };
        _taskRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
        var result = await _sut.ResolvePendingAsync(1);
        result.Should().BeTrue();
        task.Status.Should().Be(Domain.Enums.TaskStatus.Open);
    }
}
