using KafkaFlow.Producers;
using TaskManager.Application.Interfaces;

namespace TaskManager.API.Kafka;

public class KafkaProduceService : IProduceService
{
    private readonly IProducerAccessor _producers;

    public KafkaProduceService(IProducerAccessor producers)
    {
        _producers = producers ?? throw new ArgumentNullException(nameof(producers));
    }

    public async Task ProduceAsync<TMessageKey, TMessageValue>(string topic, TMessageKey key, TMessageValue value)
    {
        await _producers[topic].ProduceAsync(key, value);
    }
}