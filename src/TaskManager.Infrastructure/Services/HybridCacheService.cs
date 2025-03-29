using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Interfaces;
using ZiggyCreatures.Caching.Fusion;

namespace TaskManager.Infrastructure.Services;

public class HybridCacheService : ICacheService
{
    private readonly IFusionCache _cache;

    public HybridCacheService(IFusionCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }
    
    public async Task SetAsync<T>(string key, T obj, TimeSpan time, CancellationToken cancellationToken = default)
    {
        await _cache.SetAsync(
            key,
            JsonSerializer.Serialize(obj),
            new FusionCacheEntryOptions(time),
            cancellationToken);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        string? value = await _cache.GetOrDefaultAsync<string>(key, token: cancellationToken);
        if (string.IsNullOrEmpty(value))
            return default;
        T? obj = JsonSerializer.Deserialize<T>(value);
        return obj;
    }
    
    public async Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default)
    {
        string? value = await _cache.GetOrDefaultAsync<string>(key, token: cancellationToken);
        return value;
    }
    
    public async Task SetStringAsync(string key, string obj, TimeSpan time, CancellationToken cancellationToken = default)
    {
        await _cache.SetAsync(
            key,
            obj,
            new FusionCacheEntryOptions(time),
            cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, token: cancellationToken);
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