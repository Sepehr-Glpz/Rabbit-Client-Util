using System.IO;
using System.Text.Json;
using System.Threading;

namespace SGSX.RabbitClient.Core;
internal class JsonMessageSerializer : IMessageSerializer
{
    private readonly JsonSerializerOptions _defaults = new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public TMessage Deserialize<TMessage>(ReadOnlyMemory<byte> data)
    {
        return JsonSerializer.Deserialize<TMessage>(new MemoryStream(data.ToArray()), _defaults) ??
            throw BadFormat();
    }

    public async Task<TMessage> DeserializeAsync<TMessage>(ReadOnlyMemory<byte> data, CancellationToken ct)
    {
        return await JsonSerializer.DeserializeAsync<TMessage>(new MemoryStream(data.ToArray()), _defaults, ct) ??
            throw BadFormat();
    }

    public Memory<byte> Serialize<TMessage>(TMessage message)
    {
        using var memoryStream = new MemoryStream();
        JsonSerializer.Serialize<TMessage>(memoryStream, message, _defaults);
        return memoryStream.ToArray().AsMemory();
    }

    public async Task<Memory<byte>> SerializeAsync<TMessage>(TMessage message, CancellationToken ct)
    {
        using var memoryStream = new MemoryStream();
        await JsonSerializer.SerializeAsync<TMessage>(memoryStream, message, _defaults, ct);
        return memoryStream.ToArray().AsMemory();
    }

    private static FormatException BadFormat() => new("failed to deserialize message");
}
