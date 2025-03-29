using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;

namespace TaskManager.Application.Commands.CreateTask;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Result>
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<CreateTaskCommandHandler> _logger;
    private readonly IValidator<CreateTaskCommand> _validator;

    public CreateTaskCommandHandler(ITaskRepository repository, ILogger<CreateTaskCommandHandler> logger, IValidator<CreateTaskCommand> validator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _validator = validator ?? new CreateTaskCommandValidator();
    }

    public async Task<Result> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Invalid(validationResult.AsErrors());
        }
        await _repository.Create(request.Task, cancellationToken);
        _logger.LogInformation($"Task {request.Task.Name} has been created.");
        return Result.Success();
    }
}