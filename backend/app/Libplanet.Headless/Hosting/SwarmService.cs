using Libplanet.Net;
using Microsoft.Extensions.Hosting;

namespace Libplanet.Headless.Hosting;

public class SwarmService : BackgroundService, IDisposable
{
    public enum BootstrapMode
    {
        Seed,
        Participant,
    }

    private readonly Swarm _swarm;
    private readonly SwarmService.BootstrapMode _mode;

    public SwarmService(Swarm swarm, SwarmService.BootstrapMode mode)
    {
        _swarm = swarm;
        _mode = mode;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_mode != BootstrapMode.Seed)
        {
            await _swarm.BootstrapAsync(cancellationToken: stoppingToken)
                .ConfigureAwait(false);
        }

        await _swarm.PreloadAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
        await _swarm.StartAsync(cancellationToken: stoppingToken).ConfigureAwait(false);
    }
}
