using SGSX.RabbitClient.Core;
using SGSX.RabbitClient.Interfaces;

namespace SGSX.RabbitClient.Handler;
public abstract class AsyncHandlerBase : IAsyncHandler
{
    protected AsyncHandlerBase()
    {
        Serializer = new JsonMessageSerializer();
    }

    public IMessageSerializer Serializer { get; }

    public abstract Task<HandleResult> HandleAsync(HandleArgs args);
}
