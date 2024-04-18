using RabbitMQ.Client.Events;
using SGSX.RabbitClient.Interfaces;
using System.Threading;

namespace SGSX.RabbitClient.Handler;
public readonly struct HandleArgs(IMessageSerializer serializer, BasicDeliverEventArgs args)
{
    private readonly IMessageSerializer _serializer = serializer;
    private readonly BasicDeliverEventArgs _args = args;

    public IReadOnlyDictionary<string, object> GetHeaders() => _args.BasicProperties.Headers.AsReadOnly();
    public TMessage BodyAs<TMessage>() => 
        _serializer.Deserialize<TMessage>(_args.Body);
    public Task<TMessage> BodyAsAsync<TMessage>(CancellationToken ct = default) => 
        _serializer.DeserializeAsync<TMessage>(_args.Body, ct);
    public Memory<byte> BodyRaw() => _args.Body.ToArray().AsMemory();
}
