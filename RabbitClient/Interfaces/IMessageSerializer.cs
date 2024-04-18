
using System.Threading;

namespace SGSX.RabbitClient;
public interface IMessageSerializer
{
    TMessage Deserialize<TMessage>(ReadOnlyMemory<byte> data);
    Memory<byte> Serialize<TMessage>(TMessage message);

    Task<TMessage> DeserializeAsync<TMessage>(ReadOnlyMemory<byte> data, CancellationToken ct);
    Task<Memory<byte>> SerializeAsync<TMessage>(TMessage message, CancellationToken ct);
}
