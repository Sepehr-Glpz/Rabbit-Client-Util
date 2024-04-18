using SGSX.RabbitClient.Handler;

namespace SGSX.RabbitClient.Interfaces;
public interface IHandler
{
    HandleResult Handle(HandleArgs args);

    IMessageSerializer Serializer { get; }
}
