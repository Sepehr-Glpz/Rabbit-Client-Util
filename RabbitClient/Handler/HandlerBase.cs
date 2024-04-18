using SGSX.RabbitClient.Core;

namespace SGSX.RabbitClient.Handler;
public abstract class HandlerBase : IHandler
{
    protected HandlerBase()
    {
        Serializer = new JsonMessageSerializer();
    }

    public IMessageSerializer Serializer { get; }

    public abstract HandleResult Handle(HandleArgs args);
}
