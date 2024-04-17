using RabbitMQ.Client.Events;
using SGSX.RabbitClient.Interfaces;
using System.Threading;

namespace SGSX.RabbitClient.Handler;
public readonly struct HandleArgs(IAsyncHandler handler, BasicDeliverEventArgs args)
{
    private readonly IAsyncHandler _handler = handler;
    private readonly BasicDeliverEventArgs _args = args;

    public IReadOnlyDictionary<string, object> GetHeaders() => _args.BasicProperties.Headers.AsReadOnly();
    public TMessage BodyAs<TMessage>() => 
        _handler.Serializer.Deserialize<TMessage>(_args.Body);
    public Task<TMessage> BodyAsAsync<TMessage>(CancellationToken ct = default) => 
        _handler.Serializer.DeserializeAsync<TMessage>(_args.Body, ct);
    public Memory<byte> BodyRaw() => _args.Body.ToArray().AsMemory();
}
