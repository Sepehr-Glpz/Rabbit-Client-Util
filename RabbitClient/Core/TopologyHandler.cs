using SGSX.RabbitClient.Configuration;
using SGSX.RabbitClient.Connection;
using Microsoft.Extensions.Options;

namespace SGSX.RabbitClient.Core;
internal class TopologyHandler(ConnectionHandler connection, IOptions<MappingConfig> mapping) : ITopology
{
    protected ConnectionHandler Connection { get; } = connection;
    protected MappingConfig Mapping { get; } = mapping.Value;

    protected const string ConfigChannelKey = "rabbit-mapping";

    public void CommitAll(bool noWait = false)
    {
        CommitExchanges(noWait);

        CommitQueues(noWait);

        CommitBindings(noWait);

        Connection.Channels.CloseChannel(ConfigChannelKey);
    }

    public void CommitQueues(bool noWait)
    {
        if (Mapping.Queues is not { Count: > 0 })
            return;

        var channel = Connection.Channels.GetChannel(ConfigChannelKey);


        foreach (var queue in Mapping.Queues)
        {
            if (!noWait)
                channel.QueueDeclare(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
            else
                channel.QueueDeclareNoWait(queue.Name, queue.Durable, queue.Exclusive, queue.AutoDelete, queue.Arguments);
        }
    }

    public void CommitExchanges(bool noWait)
    {
        if (Mapping.Exchanges is not { Count: > 0 })
            return;

        var channel = Connection.Channels.GetChannel(ConfigChannelKey);


        foreach (var exchange in Mapping.Exchanges)
        {
            if (!noWait)
                channel.ExchangeDeclare(exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Arguments);
            else
                channel.ExchangeDeclareNoWait(exchange.Name, exchange.Type, exchange.Durable, exchange.AutoDelete, exchange.Arguments);
        }
    }

    public void CommitBindings(bool noWait)
    {
        if (Mapping.Bindings is not { Count: > 0 })
            return;

        var channel = Connection.Channels.GetChannel(ConfigChannelKey);


        foreach (var binding in Mapping.Bindings)
        {
            var from = Mapping.Queues!.First(c => c.Id == binding.FromQueueId);
            var to = Mapping.Exchanges!.First(c => c.Id == binding.ToExchangeId);

            foreach (var key in binding.Keys)
                if (!noWait)
                    channel.QueueBind(from.Name, to.Name, key, binding.Arguments);
                else
                    channel.QueueBindNoWait(from.Name, to.Name, key, binding.Arguments);
        }
    }

}
