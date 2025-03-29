namespace TaskManager.Application.Interfaces;

public interface ICacheService
{
    Task SetAsync<T>(string key, T obj, TimeSpan time, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);
    Task SetStringAsync(string key, string obj, TimeSpan time, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    Task<T?> GetOrSetAsync<T>(string key, T obj, TimeSpan time, CancellationToken cancellationToken = default)
        where T : class;
}