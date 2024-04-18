using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SGSX.RabbitClient.Connection;
using SGSX.RabbitClient.Handler;
using SGSX.RabbitClient.Interfaces;

namespace SGSX.RabbitClient.Core;
internal class Consumer(ConnectionHandler connection, HandlerFactory handlerFactory, IServiceProvider serviceProvider) : IConsumer
{
    #region Fields

    private readonly IServiceProvider _serviceProvider = serviceProvider;
    protected HandlerFactory HandlerFactory { get; } = handlerFactory;
    protected ConnectionHandler Connection { get; } = connection;

    #endregion

    #region Methods

    public IBasicConsumer Consume(string group, string queue, string? consumerTag, bool exclusive, IDictionary<string, object>? args)
    {
        var channel = KeysThreadChannel($"consumer-{queue}");

        var consumer = CreateConsumer(group, channel);

        channel.BasicConsume(consumer: consumer, queue: queue, autoAck: false, consumerTag: consumerTag, exclusive: exclusive, arguments: args);

        return consumer;
    }

    private AsyncEventingBasicConsumer CreateConsumer(string group, IModel channel)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (sender, args) =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var asyncHandlers = HandlerFactory.GetAsyncHandlers(group, scope.ServiceProvider);
                var handlers = HandlerFactory.GetHandlers(group, scope.ServiceProvider);

                var asyncHandles = asyncHandlers
                    .Select(s => s.HandleAsync(new HandleArgs(s.Serializer, args)));
                var handles = handlers
                    .Select(s => Task.Factory.StartNew(() => s.Handle(new HandleArgs(s.Serializer, args))));
                    
                var results = await Task.WhenAll(Enumerable.Union(asyncHandles, handles));

                var resultSum = results.Aggregate(HandleResult.Undefined, (prev, current) => prev | current);

                if ((resultSum & HandleResult.Nack) == HandleResult.Nack)
                {
                    bool requeue = (resultSum & HandleResult.Requeue) == HandleResult.Requeue;

                    channel.BasicNack(args.DeliveryTag, false, requeue);
                }
                else if ((resultSum & HandleResult.Ack) == HandleResult.Ack)
                {
                    channel.BasicAck(args.DeliveryTag, false);
                }
            }
            catch
            {
                channel.BasicReject(args.DeliveryTag, true);
            }
        };
        return consumer;
    }

    private IModel KeysThreadChannel(string key) =>
        Connection.Channels.GetChannel($"{key}-{Environment.CurrentManagedThreadId}");

    #endregion
}
