using Microsoft.Extensions.DependencyInjection;
using SGSX.RabbitClient.Attributes;
using System.Reflection;

namespace SGSX.RabbitClient.Handler;
internal class HandlerFactory(IEnumerable<Type> handlerTypes)
{
    #region Fields

    protected IReadOnlyDictionary<string, IEnumerable<Type>> GroupHandlers { get; } = CreateGroupHandlerTypeMap(handlerTypes);

    #endregion

    #region Methods

    public IEnumerable<IAsyncHandler> GetAsyncHandlers(string group, IServiceProvider provider) =>
            GroupHandlers.GetValueOrDefault(group)
                ?.Where(type => type.IsAssignableTo(typeof(IAsyncHandler)))
                .Select(provider.GetRequiredService)
                .Cast<IAsyncHandler>() ?? [];

    public IEnumerable<IHandler> GetHandlers(string group, IServiceProvider provider) =>
            GroupHandlers.GetValueOrDefault(group)
                ?.Where(type => type.IsAssignableTo(typeof(IHandler)))
                .Select(provider.GetRequiredService)
                .Cast<IHandler>() ?? [];

    private static Dictionary<string, IEnumerable<Type>> CreateGroupHandlerTypeMap(IEnumerable<Type> types)
    {
        var map = new Dictionary<string, IEnumerable<Type>>();
        foreach (var type in types)
        {
            if (!(type.IsAssignableTo(typeof(IAsyncHandler)) || type.IsAssignableTo(typeof(IHandler))))
                continue;

            foreach (var att in type.GetCustomAttributes<HandlerGroupAttribute>())
            {
                var current = map.GetValueOrDefault(att.Group) ?? [];

                map[att.Group] = current.Append(type);
            }
        }
        return map;
    }

    #endregion
}
