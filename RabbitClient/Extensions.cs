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
    public static void AddRabbitMQ(this IServiceCollection services, IConfiguration configuration, bool asyncConsumers, params Assembly[] assemblies)
    {
        services.AddRabbitConfiguration(configuration, asyncConsumers);
        services.AddRabbitConnection();
        services.AddRabbitTopology();
        services.AddRabbitPublisher();
        services.AddRabbitConsumers(assemblies);
    }

    private const string CONFIG_SECTION = "RabbitMQ";
    private const string CONNECTION_SECTION = "Connection";
    private const string MAPPING_SECTION = "Mapping";

    private static void AddRabbitConfiguration(this IServiceCollection services, IConfiguration configuration, bool asyncConsumers)
    {
        var rabbitSection = configuration.GetRequiredSection(CONFIG_SECTION);

        services.Configure<ConnectionConfig>(opt =>
        {
            var conn = rabbitSection.GetRequiredSection(CONNECTION_SECTION).Get<ConnectionConfig>()!;
            conn.AsyncConsumers = asyncConsumers;
            opt += conn;
        });

        services.Configure<MappingConfig>(rabbitSection.GetRequiredSection(MAPPING_SECTION));
    }
    private static void AddRabbitConnection(this IServiceCollection services)
    {
        services.AddSingleton<ConnectionHandler>();
        services.AddSingleton<IRabbitConnection, ConnectionHandler>();
    }
    private static void AddRabbitConsumers(this IServiceCollection services, Assembly[] assemblies)
    {
        services.AddSingleton<Consumer>();
        services.AddSingleton<IConsumer, Consumer>();

        services.AddSingleton<HandlerFactory>(sp =>
        {
            var scanning = assemblies
                .SelectMany(asm => asm.DefinedTypes
                .Where(t => t.IsAssignableTo(typeof(IAsyncHandler))));

            return new HandlerFactory(scanning);
        });
    }
    private static void AddRabbitPublisher(this IServiceCollection services)
    {
        services.AddSingleton<Publisher>();
        services.AddSingleton<IPublisher, Publisher>();
    }
    private static void AddRabbitTopology(this IServiceCollection services)
    {
        services.AddSingleton<TopologyHandler>();
        services.AddSingleton<ITopology, TopologyHandler>();
    }
}
