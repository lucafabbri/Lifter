using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;

namespace Lifter.Core.WatchDog;

/// <summary>
/// Internal implementation of the WatchDog service. Manages the lifecycle, policies, and state of all registered hosted services.
/// </summary>
internal sealed class WatchDogService : IHostManagerWatchDog, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, HostedServiceState> _serviceStates = new();
    private readonly object _lock = new();
    private Timer? _restartTimer;

    public event Action<HostedServiceState>? StatusChanged;

    public WatchDogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    internal void Initialize()
    {
        var allServices = _serviceProvider.GetServices<IHostedService>();
        var optionProviders = _serviceProvider.GetServices<ServiceOptionsProvider>()
            .ToDictionary(p => p.ServiceType, p => p.Options);

        foreach (var service in allServices)
        {
            // Correction for CS0019: Use ReferenceEquals to safely compare instances
            // to ensure we don't try to monitor the WatchDog service itself.
            if (object.ReferenceEquals(service, this)) continue;

            var serviceType = service.GetType();
            var options = optionProviders.GetValueOrDefault(serviceType, new HostedServiceOptions());
            _serviceStates.TryAdd(serviceType, new HostedServiceState(service, options));
        }
    }

    internal Task StartMonitoringAsync(CancellationToken cancellationToken)
    {
        var servicesToStart = _serviceStates.Values
            .Where(s => s.Options.Startup == StartupPolicy.Automatic && s.Status == ServiceStatus.Stopped)
            .Select(s => StartServiceAsync(s.Instance.GetType(), cancellationToken));

        _restartTimer = new Timer(CheckForRestarts, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

        return Task.WhenAll(servicesToStart);
    }

    internal Task StopMonitoringAsync(CancellationToken cancellationToken)
    {
        _restartTimer?.Change(Timeout.Infinite, 0);
        var servicesToStop = _serviceStates.Values
            .Where(s => s.Status is ServiceStatus.Running or ServiceStatus.Restarting)
            .Select(s => StopServiceAsync(s.Instance.GetType(), cancellationToken));

        return Task.WhenAll(servicesToStop);
    }

    public IReadOnlyDictionary<Type, HostedServiceState> GetAllStatuses() => _serviceStates;

    public HostedServiceState? GetStatus(Type serviceType) => _serviceStates.GetValueOrDefault(serviceType);

    public Task StartServiceAsync(Type serviceType, CancellationToken cancellationToken = default)
    {
        if (!_serviceStates.TryGetValue(serviceType, out var state))
        {
            return Task.CompletedTask;
        }

        lock (_lock)
        {
            if (state.Status is not ServiceStatus.Stopped and not ServiceStatus.Failed)
            {
                return Task.CompletedTask;
            }
            state.TransitionTo(ServiceStatus.Starting);
        }

        NotifyStatusChange(state);

        return ExecuteStartAsync(state, cancellationToken);
    }

    public async Task StopServiceAsync(Type serviceType, CancellationToken cancellationToken = default)
    {
        if (!_serviceStates.TryGetValue(serviceType, out var state)) return;

        lock (_lock)
        {
            if (state.Status is not ServiceStatus.Running and not ServiceStatus.Restarting) return;
            state.TransitionTo(ServiceStatus.Stopping);
        }

        NotifyStatusChange(state);

        try
        {
            await state.Instance.StopAsync(cancellationToken);
            state.TransitionTo(ServiceStatus.Stopped);
        }
        catch (Exception ex)
        {
            state.TransitionTo(ServiceStatus.Failed, ex);
        }
        finally
        {
            NotifyStatusChange(state);
        }
    }

    private async Task ExecuteStartAsync(HostedServiceState state, CancellationToken cancellationToken)
    {
        try
        {
            await state.Instance.StartAsync(cancellationToken);
            state.TransitionTo(ServiceStatus.Running);
        }
        catch (Exception ex)
        {
            state.TransitionTo(ServiceStatus.Failed, ex);
        }
        finally
        {
            NotifyStatusChange(state);
        }
    }

    private void CheckForRestarts(object? state)
    {
        var servicesToRestart = _serviceStates.Values.Where(s =>
                s.Status == ServiceStatus.Failed &&
                s.Options.Restart == RestartPolicy.OnFailure &&
                s.Options.MaxRestartAttempts > s.CurrentRestartAttempts &&
                (DateTime.UtcNow - s.LastStatusChangeUtc) > s.Options.RestartDelay)
            .ToList();

        foreach (var serviceState in servicesToRestart)
        {
            lock (_lock)
            {
                if (serviceState.Status != ServiceStatus.Failed) continue;
                serviceState.TransitionTo(ServiceStatus.Restarting);
            }

            NotifyStatusChange(serviceState);
            _ = ExecuteStartAsync(serviceState, default);
        }
    }

    private void NotifyStatusChange(HostedServiceState state) => StatusChanged?.Invoke(state);

    public void Dispose()
    {
        _restartTimer?.Dispose();
    }
}

