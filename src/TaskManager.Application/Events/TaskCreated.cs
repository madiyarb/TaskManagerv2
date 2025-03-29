using TaskManager.Domain;
using TaskManager.Domain.DomainModels;

namespace TaskManager.Application.Events;

public class TaskCreated : ITaskEvent
{
    public string Type => nameof(TaskCreated);
    public TaskDomainModel Task { get; set; }
}