using System.Runtime.CompilerServices;

namespace SGSX.RabbitClient.Configuration;

internal class ConnectionConfig
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string VirtualHost { get; set; }
    public string? DefaultHost { get; set; }
    public ushort? DefaultPort { get; set; } = 5672;
    public required List<Endpoint> Endpoints { get; set; }

    internal class Endpoint
    {
        public string? Host { get; set; }
        public ushort? Port { get; set; }
    }
}


