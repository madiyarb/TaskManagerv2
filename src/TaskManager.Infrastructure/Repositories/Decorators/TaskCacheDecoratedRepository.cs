using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.DomainModels;
using TaskManager.Infrastructure.Interfaces;

namespace TaskManager.Infrastructure.Repositories.Decorators;

public class TaskCacheDecoratedRepository : ITaskRepository
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TaskCacheDecoratedRepository> _logger;
    private readonly ICacheService _cache;
    private readonly ITaskUnitOfWork _taskUnitOfWork;

    public TaskCacheDecoratedRepository(
        ITaskRepository repository,
        ILogger<TaskCacheDecoratedRepository> logger,
        ICacheService cache,
        ITaskUnitOfWork taskUnitOfWork)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _taskUnitOfWork = taskUnitOfWork ?? throw new ArgumentNullException(nameof(taskUnitOfWork));
    }

    public async Task Create(TaskDomainModel task, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Create(task, cancellationToken);
            string cacheIdKey = $"Task_Id_{task.Id}";
            string cachePhoneKey = $"Task_Phone_{task.Id}";
            await _cache.SetStringAsync(
                cacheIdKey,
                JsonSerializer.Serialize(task),
                TimeSpan.FromMinutes(10),
                cancellationToken);
            await _cache.SetStringAsync(
                cachePhoneKey,
                cacheIdKey,
                TimeSpan.FromDays(5));
            _logger.LogInformation(@"Task {task} has been created.", task);
        }
        catch (Exception e) when (e is InvalidOperationException
                                  || e is DbUpdateException
                                  || e is DbUpdateConcurrencyException)
        {
            _logger.LogInformation("Task {@task} has not been created. Exception: {@exception}", task, e);
            throw;
        }
    }

    public async Task Update(TaskDomainModel task, CancellationToken cancellationToken)
    {
        string cacheIdKey = $"Task_Id_{task.Id}";
        try
        {
            await _repository.Update(task, cancellationToken);
            await _cache.SetStringAsync(cacheIdKey,
                JsonSerializer.Serialize(task),
                TimeSpan.FromMinutes(10),
                cancellationToken);
            _logger.LogInformation("Task {@task} has been updated.", task);
        }
        catch (RedisException e)
        {
            await _cache.RemoveAsync(cacheIdKey);
            _logger.LogInformation($"Task {@task} has  been removed from cache. Exception: {@e}", task, e);
            throw;
        }
        catch (Exception e) when (e is InvalidOperationException
                                  || e is DbUpdateException
                                  || e is DbUpdateConcurrencyException)
        {
            _logger.LogInformation("Task {@task} has not been updated. Exception: {@exception}", task, e);
            throw;
        }
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