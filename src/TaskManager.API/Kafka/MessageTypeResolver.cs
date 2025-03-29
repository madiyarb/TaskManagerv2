using System.Reflection;
using KafkaFlow;
using KafkaFlow.Middlewares.Serializer.Resolvers;
using TaskManager.Domain;

namespace TaskManager.API.Kafka;

public class MessageTypeResolver : IMessageTypeResolver
{
    private const string MessageType = "Message-Type";
    public ValueTask OnProduceAsync(IMessageContext context)
    {

        string type = context.Message.Value.GetType().Name;
        context.Headers.SetString(MessageType, type);
        return new ValueTask();
    }

    public ValueTask<Type> OnConsumeAsync(IMessageContext context)
    {
        var messageTypeName = context.Headers?.GetString(MessageType);

        if (messageTypeName == null)
            return new ValueTask<Type>(typeof(ITaskEvent));

        var coreType = typeof(ITaskEvent);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => coreType.IsAssignableFrom(p));

        foreach (var type in types)
        {
            if (messageTypeName == type.Name)
            {
                return new ValueTask<Type>(type);
            }
        }

        return new ValueTask<Type>(typeof(ITaskEvent));
    }
    public Type OnConsume(IMessageContext context)
    {
        throw new NotImplementedException();
    }

    public void OnProduce(IMessageContext context)
    {
        throw new NotImplementedException();
    }

    private static List<Assembly> GetListOfEntryAssemblyWithReferences()
    {
        List<Assembly> listOfAssemblies = new List<Assembly>();
        var mainAsm = Assembly.GetEntryAssembly();
        listOfAssemblies.Add(mainAsm);

        foreach (var refAsmName in mainAsm.GetReferencedAssemblies())
        {
            listOfAssemblies.Add(Assembly.Load(refAsmName));
        }
        return listOfAssemblies;
    }
}
