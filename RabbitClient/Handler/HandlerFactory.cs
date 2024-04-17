using SGSX.RabbitClient.Attributes;
using SGSX.RabbitClient.Interfaces;
using System.Reflection;

namespace SGSX.RabbitClient.Handler;
internal class HandlerFactory
{
    #region Constructor

    public HandlerFactory(IEnumerable<Type> handlerTypes, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        QueueAsyncHandlers = CreateQueueAsyncHandlerTypeMap(handlerTypes);
    }

    #endregion

    #region Fields

    private readonly IServiceProvider _serviceProvider;
    protected IReadOnlyDictionary<string, IEnumerable<Type>> QueueAsyncHandlers { get; }

    #endregion

    #region Methods

    public IEnumerable<IAsyncHandler> GetAsyncHandlers(string queue)
    {
        var types = QueueAsyncHandlers[queue];

        foreach (var type in types)
        {
            if(_serviceProvider.GetService(type) is IAsyncHandler handler)
                yield return handler;
        }
    }

    private static Dictionary<string, IEnumerable<Type>> CreateQueueAsyncHandlerTypeMap(IEnumerable<Type> types)
    {
        var map = new Dictionary<string, IEnumerable<Type>>();
        foreach(var type in types)
        {
            if (type.IsAssignableTo(typeof(IAsyncHandler)))
                continue;

            foreach(var att in type.GetCustomAttributes<HandleQueueAttribute>())
            {
                var current = map[att.Queue] ?? [];

                map[att.Queue] = current.Append(type);
            }
        }
        return map;
    }

    #endregion
}
