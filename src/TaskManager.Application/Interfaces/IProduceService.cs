namespace TaskManager.Application.Interfaces;

public interface IProduceService
{
    Task ProduceAsync<TMessageKey, TMessageValue>(string topic, TMessageKey key, TMessageValue value);
}