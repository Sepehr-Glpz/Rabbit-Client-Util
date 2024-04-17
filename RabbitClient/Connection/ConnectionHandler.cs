using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SGSX.RabbitClient.Configuration;
using SGSX.RabbitClient.Interfaces;
using System.Threading;

namespace SGSX.RabbitClient.Connection;
internal class ConnectionHandler : IRabbitConnection
{
    #region Constructor

    public ConnectionHandler(IOptions<ConnectionConfig> config) =>
        (Config, Channels, OnDisconnect) = (config.Value, new(this), new Action(() => { }));

    #endregion

    #region Fields

    protected IConnection? ConnectionInstance { get; set; }

    public IConnection InnerConnection => IsConnected switch
    {
        true => ConnectionInstance!,
        false => throw new InvalidOperationException("Client is not Connected!"),
    };

    protected ConnectionConfig Config { get; }

    public ChannelHandler Channels { get; }

    public event Action OnDisconnect;

    public bool IsConnected => ConnectionInstance?.IsOpen ?? false;

    public bool IsAsyncConsumeMode => Config.AsyncConsumers;

    #endregion

    #region Methods

    public Task ConnectAsync(CancellationToken ct) => Task.Factory.StartNew(OpenConnection, ct);
    public Task CloseAsync(CancellationToken ct) => Task.Factory.StartNew(CloseConnection, ct);

    private readonly object _syncRoot = new();
    protected virtual void OpenConnection()
    {
        lock (_syncRoot)
        {
            if (IsConnected)
                return;

            var factory = new ConnectionFactory()
            {
                HostName = Config.DefaultHost,
                Port = Config.DefaultPort ?? 0,
                UserName = Config.Username,
                Password = Config.Password,
                VirtualHost = Config.VirtualHost,
                RequestedHeartbeat = TimeSpan.FromSeconds(30),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(3),
                TopologyRecoveryEnabled = true,
                DispatchConsumersAsync = Config.AsyncConsumers,
            };

            ConnectionInstance = Config switch
            {
                { Endpoints: [] } => factory.CreateConnection(),
                { Endpoints.Count: > 0 } => factory.CreateConnection(
                    Config.Endpoints
                    .Select(endpoint => new AmqpTcpEndpoint(endpoint.Host ?? Config.DefaultHost, endpoint.Port ?? Config.DefaultPort ?? 0))
                    .ToList()
                    ),
            };

            ConnectionInstance.ConnectionShutdown += (_,_) => OnDisconnect();
        }
    }

    protected virtual void CloseConnection()
    {   
        ConnectionInstance?.Close();
        ConnectionInstance?.Dispose();

        ConnectionInstance = null;
    }

    #endregion

    #region Dispose Pattern

    public bool IsDisposed { get; protected set; } = false;

    protected void Dispose(bool disposing)
    {
        if (IsDisposed)
            return;
        if (disposing)
        {
            CloseConnection();
        }

        ConnectionInstance = null;

        IsDisposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ConnectionHandler()
    {
        Dispose(false);
    }

    #endregion
}
