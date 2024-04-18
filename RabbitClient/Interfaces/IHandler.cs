using SGSX.RabbitClient.Handler;

namespace SGSX.RabbitClient;
public interface IHandler
{
    HandleResult Handle(HandleArgs args);

    IMessageSerializer Serializer { get; }
}
