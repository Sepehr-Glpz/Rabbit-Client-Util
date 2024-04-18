using RabbitMQ.Client;
using System.Threading;

namespace SGSX.RabbitClient;
public interface IPublisher
{
    void Publish<TMessage>(TMessage message, string exchange, string routeKey, IBasicProperties? props);
    Task PublishAsync<TMessage>(TMessage message, string exchange, string routeKey, IBasicProperties? props, CancellationToken ct);
    IMessageSerializer Serializer { get; set; }
}
