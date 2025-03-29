using Microsoft.EntityFrameworkCore.Storage;

namespace TaskManager.Infrastructure.Interfaces;

public interface ITaskUnitOfWorkInfr
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

}