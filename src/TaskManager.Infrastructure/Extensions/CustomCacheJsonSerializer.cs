using System.Text.Json;
using ZiggyCreatures.Caching.Fusion.Serialization;

namespace TaskManager.Infrastructure.Extensions;

public class CustomCacheJsonSerializer 
{
    public byte[] Serialize<T>(T? obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes<T?>(obj);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(byte[] data)
    {
        return JsonSerializer.Deserialize<T>(data);
    }

    /// <inheritdoc />
    public ValueTask<byte[]> SerializeAsync<T>(T? obj, CancellationToken token = default)
    {
        return new ValueTask<byte[]>(Serialize(obj));
    }

    /// <inheritdoc />
    public ValueTask<T?> DeserializeAsync<T>(byte[] data, CancellationToken token = default)
    {
        return new ValueTask<T?>(Deserialize<T>(data));
    }
}
