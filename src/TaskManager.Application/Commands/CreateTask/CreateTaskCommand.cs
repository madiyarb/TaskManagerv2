using Ardalis.Result;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskManager.Domain.DomainModels;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Commands.CreateTask;

public class CreateTaskCommand : IRequest<Result>
{
    public TaskDomainModel Task { get; set; }

    public CreateTaskCommand()
    {
        
    }

    public CreateTaskCommand(TaskDomainModel task)
    {
        Task = task;
    }
}