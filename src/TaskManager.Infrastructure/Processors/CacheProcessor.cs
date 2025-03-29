using TaskManager.Application.Enums;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Interfaces;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure.Processors;

public class CacheProcessor : ICacheProcessor
{
    private readonly IEnumerable<ICacheService> _services;

    public CacheProcessor(IEnumerable<ICacheService> services)
    {
        _services = services;
    }

    public async Task SetAsync<T>(CacheTypeEnums type, string key, T obj, TimeSpan time,
        CancellationToken cancellationToken = default)
    {
        if (type == CacheTypeEnums.Hybrid)
        {
            var service = _services.First(s => s.GetType() == typeof(HybridCacheService));
            await service.SetAsync(key, obj, time, cancellationToken);
        }
        else if (type == CacheTypeEnums.Redis)
        {
            var service = _services.First(s => s.GetType() == typeof(RedisCacheService));
            await service.SetAsync(key, obj, time, cancellationToken);
        }
        else
            throw new KeyNotFoundException("Cache service not found");
    }

    public async Task<T?> GetAsync<T>(CacheTypeEnums type, string key, CancellationToken cancellationToken = default)
        where T : class
    {
        if (type == CacheTypeEnums.Hybrid)
        {
            var service = _services.First(s => s.GetType() == typeof(HybridCacheService));
            return await service.GetAsync<T>(key, cancellationToken);
        }
        else if (type == CacheTypeEnums.Redis)
        {
            var service = _services.First(s => s.GetType() == typeof(RedisCacheService));
            return await service.GetAsync<T>(key, cancellationToken);
        }
        else
            throw new KeyNotFoundException("Cache service not found");
    }

    public async Task<string?> GetStringAsync(CacheTypeEnums type, string key,
        CancellationToken cancellationToken = default)
    {
        if (type == CacheTypeEnums.Hybrid)
        {
            var service = _services.First(s => s.GetType() == typeof(HybridCacheService));
            return await service.GetStringAsync(key, cancellationToken);
        }
        else if (type == CacheTypeEnums.Redis)
        {
            var service = _services.First(s => s.GetType() == typeof(RedisCacheService));
            return await service.GetStringAsync(key, cancellationToken);
        }
        else
            throw new KeyNotFoundException("Cache service not found");
    }

    public async Task SetStringAsync(CacheTypeEnums type, string key, string obj, TimeSpan time,
        CancellationToken cancellationToken = default)
    {
        if (type == CacheTypeEnums.Hybrid)
        {
            var service = _services.First(s => s.GetType() == typeof(HybridCacheService));
            await service.SetStringAsync(key, obj, time, cancellationToken);
        }
        else if (type == CacheTypeEnums.Redis)
        {
            var service = _services.First(s => s.GetType() == typeof(RedisCacheService));
            await service.SetStringAsync(key, obj, time, cancellationToken);
        }
        else
            throw new KeyNotFoundException("Cache service not found");
    }

    public async Task RemoveAsync(CacheTypeEnums type, string key, CancellationToken cancellationToken = default)
    {
        if (type == CacheTypeEnums.Hybrid)
        {
            var service = _services.First(s => s.GetType() == typeof(HybridCacheService));
            await service.RemoveAsync(key, cancellationToken);
        }
        else if (type == CacheTypeEnums.Redis)
        {
            var service = _services.First(s => s.GetType() == typeof(RedisCacheService));
            await service.RemoveAsync(key, cancellationToken);
        }
        else
            throw new KeyNotFoundException("Cache service not found");
    }


    public async Task<T?> GetOrSetAsync<T>(CacheTypeEnums type, string key, T obj, TimeSpan time,
        CancellationToken cancellationToken = default) where T : class
    {
        if (type == CacheTypeEnums.Hybrid)
        {
            var service = _services.First(s => s.GetType() == typeof(HybridCacheService));
             return await service.GetOrSetAsync<T>(key, obj, time, cancellationToken);
        }
        else if (type == CacheTypeEnums.Redis)
        {
            var service = _services.First(s => s.GetType() == typeof(RedisCacheService));
           return await service.GetOrSetAsync<T>(key, obj, time, cancellationToken);
        }
        else
            throw new KeyNotFoundException("Cache service not found");
    }
}