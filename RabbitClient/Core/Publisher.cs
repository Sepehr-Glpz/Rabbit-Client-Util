using RabbitMQ.Client;
using SGSX.RabbitClient.Connection;
using SGSX.RabbitClient.Interfaces;
using System.Threading;

namespace SGSX.RabbitClient.Core;
internal class Publisher(ConnectionHandler connection) : IPublisher
{
    #region Fields

    protected ConnectionHandler Connection { get; } = connection;

    public IMessageSerializer Serializer { get; set; } = new JsonMessageSerializer();

    #endregion

    #region Methods

    public async Task PublishAsync<TMessage>(TMessage message, string exchange, string routeKey, IBasicProperties? props, CancellationToken ct)
    {
        var channel = KeysThreadChannel($"{exchange}-{routeKey}");

        var payload = await Serializer.SerializeAsync(message, ct);

        channel.BasicPublish(exchange, routeKey, props, payload);
    }

    public void Publish<TMessage>(TMessage message, string exchange, string routeKey, IBasicProperties? props)
    {
        var channel = KeysThreadChannel($"{exchange}-{routeKey}");

        var payload = Serializer.Serialize(message);

        channel.BasicPublish(exchange, routeKey, props, payload);
    }

    private IModel KeysThreadChannel(string key) =>
        Connection.Channels.GetChannel($"{key}-{Environment.CurrentManagedThreadId}");

    #endregion
}
