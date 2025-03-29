using KafkaFlow;
using MediatR;
using TaskManager.Application.Commands.CreateTask;
using TaskManager.Application.Events;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Handlers;

public class TaskCreatedEventHandler : IMessageHandler<TaskCreated>
{
    private readonly ILogger<TaskCreatedEventHandler> _logger;
    private readonly IMediator _mediator;

    public TaskCreatedEventHandler(ILogger<TaskCreatedEventHandler> logger, IMediator mediator)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task Handle(IMessageContext context, TaskCreated message)
    {
        CreateTaskCommand createTaskCommand = new CreateTaskCommand(message.Task);
        var result = await _mediator.Send(createTaskCommand, new CancellationToken());
        if (result.IsSuccess)
            _logger.LogInformation("Task {task} has been created", createTaskCommand);
        else
        {
            _logger.LogError("Task {task} has not been created. Errors: {errors}", createTaskCommand, result.Errors);
        }
    }
}