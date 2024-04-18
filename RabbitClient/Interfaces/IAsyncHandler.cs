using SGSX.RabbitClient.Handler;

namespace SGSX.RabbitClient;
public interface IAsyncHandler
{
    Task<HandleResult> HandleAsync(HandleArgs args);

    IMessageSerializer Serializer { get; }
}
