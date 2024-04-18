
namespace SGSX.RabbitClient.Configuration;
internal class BindingConfig
{
    public required int FromQueueId { get; set; }

    public required int ToExchangeId { get; set; }

    public required string[] Keys { get; set; }

    public IDictionary<string, object>? Arguments { get; set; }
}
