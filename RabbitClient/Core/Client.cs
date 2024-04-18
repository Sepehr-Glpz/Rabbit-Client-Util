using SGSX.RabbitClient.Interfaces;
using System.Threading;

namespace SGSX.RabbitClient.Core;
internal class Client(IRabbitConnection connection, ITopology topology, IConsumer consumer, IPublisher publisher) : IRabbitClient
{
    public IConsumer Consumer { get; } = consumer;

    public IPublisher Publisher { get; } = publisher;

    public IRabbitConnection Connection { get; } = connection;

    public ITopology Topology { get; } = topology;

    public Task ConnectAsync(CancellationToken ct) => Connection.ConnectAsync(ct);

    public Task DisconnectAsync(CancellationToken ct) => Connection.CloseAsync(ct);

    public void Dispose() => Connection?.Dispose();
}
