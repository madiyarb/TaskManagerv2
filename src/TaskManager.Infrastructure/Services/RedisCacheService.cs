using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Interfaces;

namespace TaskManager.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task SetStringAsync(string key, string obj, TimeSpan time, CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(
            key,
            obj,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = time });
    }
    
    public async Task SetAsync<T>(string key, T obj, TimeSpan time, CancellationToken cancellationToken = default)
    {
        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(obj),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = time });
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? value = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(value))
            return default;
        T? obj = JsonSerializer.Deserialize<T>(value);
        return obj;
    }
    
    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        string? value = await _cache.GetStringAsync(key, cancellationToken);
        return value;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }

    public async Task<T?> GetOrSetAsync<T>(string key, T obj, TimeSpan time, CancellationToken cancellationToken = default) where T : class
    {
        string? value = await GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(value))
        {
            await SetAsync<T>(key, obj, time, cancellationToken);
            return obj;
        }
        T? result = JsonSerializer.Deserialize<T>(value);
        return result; 
    }
}