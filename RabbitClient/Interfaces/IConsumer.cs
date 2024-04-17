
namespace SGSX.RabbitClient.Interfaces;
public interface IConsumer
{
    public void Consume(string group, string queue, string? consumerTag, bool exclusive, IDictionary<string, object>? args);
}
