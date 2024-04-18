
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SGSX.RabbitClient.Interfaces;
using System.Threading;

namespace SGSX.RabbitClient;
internal class StartupService(IRabbitClient client, ILogger<StartupService> logger, IServiceProvider provider,[FromKeyedServices(Extensions.ON_STARTUP_KEY)] Func<IServiceProvider, IRabbitClient, Task> startup) : IHostedService
{
    private readonly IRabbitClient _client = client;
    private readonly ILogger<StartupService> _logger = logger;
    private readonly Func<IServiceProvider, IRabbitClient, Task> _startup = startup;
    private readonly IServiceProvider _provider = provider;
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _startup(_provider, _client);
        }
        catch(Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => _client.DisconnectAsync(cancellationToken);
}
