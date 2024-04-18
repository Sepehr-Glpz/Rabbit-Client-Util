using RabbitMQ.Client;
using System.Collections.Concurrent;

namespace SGSX.RabbitClient.Connection;
internal class ChannelHandler
{
    #region Constructor

    public ChannelHandler(ConnectionHandler connection)
    {
        Connection = connection;
        Connection.OnDisconnect += () =>
        {
            CloseAll();
        };
    }

    #endregion

    #region Fields

    protected ConnectionHandler Connection { get; }
    protected ConcurrentDictionary<string, IModel> Channels { get; } = [];

    #endregion

    #region Methods

    public IModel GetChannel(string key) => Channels.GetOrAdd(key, (_) => Connection.InnerConnection.CreateModel());

    public void CloseChannel(string key)
    {
        if(Channels.TryRemove(key, out var chan))
        {
            chan.Close();
            chan.Dispose();
        }
    }

    public void CloseAll()
    {
        var chans = Channels.Values;
        Channels.Clear();
        foreach(var chan in chans)
        {
            chan.Close();
            chan.Dispose();
        }
    }

    #endregion
}
