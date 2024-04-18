using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGSX.RabbitClient.Configuration;
using SGSX.RabbitClient.Connection;
using SGSX.RabbitClient.Core;
using SGSX.RabbitClient.Handler;
using SGSX.RabbitClient.Interfaces;
using System.Reflection;

namespace SGSX.RabbitClient;
public static class Extensions
{
    public static void AddRabbitMQ(this IServiceCollection services, IConfiguration rabbitConfiguration, params Assembly[] assemblies)
    {
        services.AddRabbitConfiguration(rabbitConfiguration);
        services.AddRabbitConnection();
        services.AddRabbitTopology();
        services.AddRabbitPublisher();
        services.AddRabbitConsumers(assemblies);
    }

    internal const string ON_STARTUP_KEY = "internal-rabbit-startup";
    public static void AddRabbitMQStartup(this IServiceCollection services, Func<IServiceProvider, IRabbitClient, Task> onStartup)
    {
        services.AddKeyedSingleton(ON_STARTUP_KEY, onStartup);
        services.AddHostedService<StartupService>();
    }

    private const string CONNECTION_SECTION = "Connection";
    private const string MAPPING_SECTION = "Mapping";

    private static void AddRabbitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ConnectionConfig>(configuration.GetRequiredSection(CONNECTION_SECTION));

        services.Configure<MappingConfig>(configuration.GetRequiredSection(MAPPING_SECTION));
    }
    private static void AddRabbitConnection(this IServiceCollection services)
    {
        services.AddSingleton<ConnectionHandler>();
        services.AddSingleton<IRabbitConnection, ConnectionHandler>(sp => sp.GetRequiredService<ConnectionHandler>());
    }
    private static void AddRabbitConsumers(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton<Consumer>();
        services.AddSingleton<IConsumer, Consumer>(sp => sp.GetRequiredService<Consumer>());

        var scanning = assemblies
                .SelectMany(asm => asm.DefinedTypes
                .Where(t => t.IsAssignableTo(typeof(IAsyncHandler)) || t.IsAssignableTo(typeof(IHandler))));

        services.AddSingleton<HandlerFactory>(sp => new HandlerFactory(scanning));

        foreach (var handler in scanning)
            services.AddScoped(handler);
    }
    private static void AddRabbitPublisher(this IServiceCollection services)
    {
        services.AddSingleton<Publisher>();
        services.AddSingleton<IPublisher, Publisher>(sp => sp.GetRequiredService<Publisher>());
    }
    private static void AddRabbitTopology(this IServiceCollection services)
    {
        services.AddSingleton<TopologyHandler>();
        services.AddSingleton<ITopology, TopologyHandler>(sp => sp.GetRequiredService<TopologyHandler>());
    }
}
