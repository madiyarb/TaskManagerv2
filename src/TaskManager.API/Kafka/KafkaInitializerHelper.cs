using System.Text.Json;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.Extensions.Options;
using TaskManager.API.Handlers;

namespace TaskManager.API.Kafka;

public class KafkaInitializerHelper
{

    public static void Initialize(IServiceCollection serviceCollection, IConfiguration configuration)
    {
        string bootstrapServer = configuration.GetValue<string>("KafkaOptions:BootstrapUrl");
        string consumerGroup = configuration.GetValue<string>("KafkaOptions:ConsumerGroupName");

        
        var jsonCoreDeserializer = new JsonCoreDeserializer(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        var jsonCoreSerializer = new JsonCoreSerializer(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        serviceCollection.AddKafka(kafka => kafka
        .UseConsoleLog()
        .AddCluster(cluster => cluster
            .WithBrokers(bootstrapServer.Split(','))
            .CreateTopicIfNotExists("Tasks", 4, 1) 
            .AddConsumer(consumer => consumer
                .Topic("Tasks")
                .WithGroupId(consumerGroup)
                .WithBufferSize(4)
                .WithWorkersCount(10)
                .WithAutoOffsetReset(AutoOffsetReset.Latest)
                .AddMiddlewares(middlewares => middlewares
                    .AddDeserializer(s => jsonCoreDeserializer, r => new MessageTypeResolver())
                    .AddTypedHandlers(h =>
                    {
                         h.AddHandler<TaskCreatedEventHandler>();
                        h.WithHandlerLifetime(InstanceLifetime.Scoped);
                    })))
            .AddProducer("Tasks", producer => producer
                .AddMiddlewares(middlewares => middlewares
                    .AddSerializer(s => jsonCoreSerializer, r => new MessageTypeResolver())
                )
                .DefaultTopic("Tasks"))
        ));


        serviceCollection.AddHostedService<KafkaBusHostedService>();
    }
}
