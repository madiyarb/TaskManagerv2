using TaskManager.Application.Enums;

namespace TaskManager.Application.Interfaces;

public interface ICacheProcessor
{
    Task SetAsync<T>(CacheTypeEnums type, string key, T obj, TimeSpan time,
        CancellationToken cancellationToken = default);

    Task<T?> GetAsync<T>(CacheTypeEnums type, string key, CancellationToken cancellationToken = default)
        where T : class;

    Task<string?> GetStringAsync(CacheTypeEnums type, string key,
        CancellationToken cancellationToken = default);

    Task SetStringAsync(CacheTypeEnums type, string key, string obj, TimeSpan time,
        CancellationToken cancellationToken = default);

    Task RemoveAsync(CacheTypeEnums type, string key, CancellationToken cancellationToken = default);

    Task<T?> GetOrSetAsync<T>(CacheTypeEnums type, string key, T obj, TimeSpan time,
        CancellationToken cancellationToken = default) where T : class;
    
}