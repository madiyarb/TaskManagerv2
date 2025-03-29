using KafkaFlow;

namespace TaskManager.API.Kafka;

public class KafkaBusHostedService(IServiceProvider serviceProvider) : IHostedService
{
    private readonly IKafkaBus _kafkaBus = serviceProvider.CreateKafkaBus();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return _kafkaBus.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _kafkaBus.StopAsync();
    }
}