
namespace SGSX.RabbitClient.Configuration;
internal class ExchangeConfig
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string Type { get; set; }

    public required bool Durable { get; set; }

    public required bool AutoDelete { get; set; }

    public Dictionary<string, object>? Arguments { get; set; }
}
