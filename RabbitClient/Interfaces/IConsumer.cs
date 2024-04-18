using RabbitMQ.Client;

namespace SGSX.RabbitClient;
public interface IConsumer
{
    public IBasicConsumer Consume(string group, string queue, string consumerTag = "", bool exclusive = false, IDictionary<string, object>? args = null);
}
