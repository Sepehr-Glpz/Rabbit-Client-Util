
namespace SGSX.RabbitClient.Handler;
public enum HandleResult : byte
{
    Undefined = 0,
    Ack = 1,
    Nack = 2,
    Requeue = 4,
}
