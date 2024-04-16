namespace SGSX.RabbitClient.Configuration;

internal class BrokerConfig
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string VirtualHost { get; set; }
    public string? DefaultHost { get; set; }
    public ushort? DefaultPort { get; set; } = 5672;
    public required List<BrokerEndpoint> Endpoints { get; set; }
}

internal class BrokerEndpoint
{
    public string? Host { get; set; }
    public ushort? Port { get; set; }
}
