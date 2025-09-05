using Lifter.Core.WatchDog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Lifter.Core;

/// <summary>
/// Internal manager to discover and control the lifecycle of registered IHostedService instances.
/// This class provides backward compatibility and delegates to WatchDogService if available.
/// </summary>
public static class HostManager
{
    private static WatchDogService? _watchDog;
    private static IEnumerable<IHostedService>? _legacyHostedServices;

    public static void Initialize(IServiceProvider services)
    {
        _watchDog = services.GetService<WatchDogService>();

        if (_watchDog != null)
        {
            _watchDog.Initialize();
        }
        else
        {
            _legacyHostedServices = services.GetServices<IHostedService>();
        }
    }

    public static async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_watchDog != null)
        {
            await _watchDog.StartMonitoringAsync(cancellationToken);
        }
        else if (_legacyHostedServices is not null)
        {
            var startTasks = _legacyHostedServices.Select(s => s.StartAsync(cancellationToken));
            await Task.WhenAll(startTasks);
        }
    }

    public static async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (_watchDog != null)
        {
            await _watchDog.StopMonitoringAsync(cancellationToken);
        }
        else if (_legacyHostedServices is not null)
        {
            var stopTasks = _legacyHostedServices.Reverse().Select(s => s.StopAsync(cancellationToken));
            await Task.WhenAll(stopTasks);
        }
    }
}

