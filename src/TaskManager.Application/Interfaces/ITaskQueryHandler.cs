using TaskManager.Domain.DomainModels;

namespace TaskManager.Application.Interfaces;

public interface ITaskQueryHandler
{
    Task<TaskDomainModel?> GetByIdAsync(int id);
}