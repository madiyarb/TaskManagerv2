using Moq;
using Xunit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Commands.CreateTask;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;
using TaskManager.Domain.Enums;

public class CreateTaskCommandHandlerTests
{
    private readonly Mock<ITaskRepository> _mockTaskService;
    private readonly Mock<ILogger<CreateTaskCommandHandler>> _mockLogger;
    private readonly CreateTaskCommandHandler _handler;

    public CreateTaskCommandHandlerTests(Mock<ILogger<CreateTaskCommandHandler>> mockLogger)
    {
        _mockLogger = new Mock<ILogger<CreateTaskCommandHandler>>();
        _mockTaskService = new Mock<ITaskRepository>();
        _handler = new CreateTaskCommandHandler(_mockTaskService.Object, _mockLogger);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenTaskIsCreatedSuccessfully()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Task = new TaskDomainModel("TaskTask", "87786944750", Statuses.Created)
        };

        // Мокируем метод CreateTask так, чтобы он всегда возвращал true
        _mockTaskService.Setup(service => await service.CreateTask(It.IsAny<TaskDomainModel>(), It.IsAny<CancellationToken>())
            .ReturnsAsync());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result);
        _mockTaskService.Verify(service => service.CreateTask(command.Title, command.Description), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenTaskCreationFails()
    {
        // Arrange
        var command = new CreateTaskCommand
        {
            Title = "Test Task",
            Description = "Test Description"
        };

        // Мокируем метод CreateTask так, чтобы он всегда возвращал false
        _mockTaskService.Setup(service => service.CreateTask(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result);
        _mockTaskService.Verify(service => service.CreateTask(command.Title, command.Description), Times.Once);
    }
}
