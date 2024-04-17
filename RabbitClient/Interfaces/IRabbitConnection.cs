
using RabbitMQ.Client;
using System.Threading;

namespace SGSX.RabbitClient.Interfaces;
public interface IRabbitConnection : IDisposable
{
    Task ConnectAsync(CancellationToken ct);

    Task CloseAsync(CancellationToken ct);

    IConnection InnerConnection { get; }

    bool IsDisposed { get; }
}
