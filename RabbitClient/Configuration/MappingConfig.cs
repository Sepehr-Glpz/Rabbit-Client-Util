
namespace SGSX.RabbitClient.Configuration;
internal class MappingConfig
{
    public required List<ExchangeConfig> Exchanges { get; set; }
    public required List<QueueConfig>? Queues { get; set; }
    public required List<BindingConfig>? Bindings { get; set; }
}
