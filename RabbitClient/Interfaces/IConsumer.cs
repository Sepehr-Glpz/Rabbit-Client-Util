using RabbitMQ.Client;

namespace SGSX.RabbitClient.Interfaces;
public interface IConsumer
{
    public IBasicConsumer Consume(string group, string queue, string? consumerTag, bool exclusive, IDictionary<string, object>? args);
}
