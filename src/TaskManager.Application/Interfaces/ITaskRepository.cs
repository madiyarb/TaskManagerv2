using TaskManager.Domain.DomainModels;

namespace TaskManager.Application.Interfaces;

public interface ITaskRepository
{
    Task Create(TaskDomainModel task, CancellationToken cancellationToken);
    Task Update(TaskDomainModel task, CancellationToken cancellationToken);
    Task<TaskDomainModel?> Get(int id, CancellationToken cancellationToken);
    Task<TaskDomainModel?> GetByName(string name, CancellationToken cancellationToken);
    Task<TaskDomainModel?> GetByPhone(string phone, CancellationToken cancellationToken);
}