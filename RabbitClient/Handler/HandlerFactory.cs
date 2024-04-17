using SGSX.RabbitClient.Attributes;
using SGSX.RabbitClient.Interfaces;
using System;
using System.Reflection;

namespace SGSX.RabbitClient.Handler;
internal class HandlerFactory(IEnumerable<Type> handlerTypes)
{
    #region Fields

    protected IReadOnlyDictionary<string, IEnumerable<Type>> GroupAsyncHandlers { get; } = CreateQueueAsyncHandlerTypeMap(handlerTypes);

    #endregion

    #region Methods

    public IEnumerable<IAsyncHandler> GetAsyncHandlers(string group, IServiceProvider provider) =>
            GroupAsyncHandlers[group]
                .Where(type => type is IAsyncHandler)
                .Select(type => provider.GetService(type))
                .Cast<IAsyncHandler>();

    private static Dictionary<string, IEnumerable<Type>> CreateQueueAsyncHandlerTypeMap(IEnumerable<Type> types)
    {
        var map = new Dictionary<string, IEnumerable<Type>>();
        foreach (var type in types)
        {
            if (type.IsAssignableTo(typeof(IAsyncHandler)))
                continue;

            foreach (var att in type.GetCustomAttributes<HandlerGroupAttribute>())
            {
                var current = map[att.Group] ?? [];

                map[att.Group] = current.Append(type);
            }
        }
        return map;
    }

    #endregion
}
