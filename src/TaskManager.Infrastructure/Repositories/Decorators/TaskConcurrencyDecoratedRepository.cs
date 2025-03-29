using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;
using TaskManager.Infrastructure.Interfaces;

namespace TaskManager.Infrastructure.Repositories.Decorators;

public class TaskConcurrencyDecoratedRepository : ITaskRepository
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TaskConcurrencyDecoratedRepository> _logger;
    private readonly ICacheService _cache;
    private readonly int _maxRetryAttempts = 3;
    private readonly TimeSpan _pauseBetweenFailures = TimeSpan.FromSeconds(2);

    public TaskConcurrencyDecoratedRepository(
        ITaskRepository repository,
        ILogger<TaskConcurrencyDecoratedRepository> logger,
        ICacheService cache)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task Create(TaskDomainModel task, CancellationToken cancellationToken)
    {
        var retryPolicy = Policy
            .Handle<DBConcurrencyException>()
            .RetryAsync(3,
                (exception, retryCount) =>
                {
                    _logger.LogInformation($"RetryCheck {retryCount} due to {exception.GetType().Name}");
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await _repository.Create(task, cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await ex.Entries.Single().ReloadAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task Update(TaskDomainModel task, CancellationToken cancellationToken)
    {
        var retryPolicy = Policy
            .Handle<DBConcurrencyException>()
            .RetryAsync(3,
                (exception, retryCount) =>
                {
                    _logger.LogInformation($"RetryCheck {retryCount} due to {exception.GetType().Name}");
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                await _repository.Create(task, cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseValues = entry.GetDatabaseValues();
                if (databaseValues == null)
                {
                    throw new InvalidOperationException("The product has been deleted by another user.");
                }
                else
                {
                    entry.OriginalValues.SetValues(databaseValues);
                }

                throw;
            }
        });
    }

    public async Task<TaskDomainModel?> Get(int id, CancellationToken cancellationToken)
    {
        return await _repository.Get(id, cancellationToken);
    }

    public async Task<TaskDomainModel?> GetByName(string name, CancellationToken cancellationToken)
    {
        return await _repository.GetByName(name, cancellationToken);
    }

    public async Task<TaskDomainModel?> GetByPhone(string phone, CancellationToken cancellationToken)
    {
        return await _repository.GetByPhone(phone, cancellationToken);
    }
}