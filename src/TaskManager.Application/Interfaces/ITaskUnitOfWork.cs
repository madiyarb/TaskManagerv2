using Microsoft.EntityFrameworkCore.Storage;

namespace TaskManager.Application.Interfaces;

public interface ITaskUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}