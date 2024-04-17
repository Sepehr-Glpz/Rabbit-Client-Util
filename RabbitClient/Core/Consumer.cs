using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SGSX.RabbitClient.Connection;
using SGSX.RabbitClient.Handler;
using SGSX.RabbitClient.Interfaces;

namespace SGSX.RabbitClient.Core;
internal class Consumer(ConnectionHandler connection, HandlerFactory handlerFactory) : IConsumer
{
    #region Fields

    protected HandlerFactory HandlerFactory { get; } = handlerFactory;
    protected ConnectionHandler Connection { get; } = connection;

    #endregion

    #region Methods

    public void Consume(string queue, string? consumerTag, bool exclusive, IDictionary<string, object>? args)
    {
        var channel = KeysThreadChannel(queue);

        IBasicConsumer consumer = Connection.IsAsyncConsumeMode switch
        {
            true => CreateAsyncConsumer(queue, channel),
            _ => throw new NotImplementedException(),
        };

        channel.BasicConsume(consumer: consumer, queue: queue, autoAck: false, consumerTag: consumerTag, exclusive: exclusive, arguments: args);
    }

    private AsyncEventingBasicConsumer CreateAsyncConsumer(string queue, IModel channel)
    {
        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (sender, args) =>
        {
            try
            {
                var handlers = HandlerFactory.GetAsyncHandlers(queue);

                var results = await Task.WhenAll(handlers.Select(s => s.HandleAsync(new HandleArgs(s, args))));

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
