using SGSX.RabbitClient.Handler;

namespace SGSX.RabbitClient.Interfaces;
public interface IAsyncHandler
{
    Task<HandleResult> HandleAsync(HandleArgs args);

    IMessageSerializer Serializer { get; }
}
