namespace Lifter.Core.WatchDog;

/// <summary>
/// Provides an interface to monitor and interact with hosted services managed by Lifter.
/// </summary>
public interface IHostManagerWatchDog
{
    /// <summary>
    /// Fires whenever the status of a monitored service changes.
    /// </summary>
    event Action<HostedServiceState> StatusChanged;

    /// <summary>
    /// Gets a read-only dictionary of all monitored services and their current state.
    /// </summary>
    IReadOnlyDictionary<Type, HostedServiceState> GetAllStatuses();

    /// <summary>
    /// Gets the current state of a specific hosted service.
    /// </summary>
    /// <param name="serviceType">The type of the hosted service.</param>
    /// <returns>The state of the service, or null if not found.</returns>
    HostedServiceState? GetStatus(Type serviceType);

    /// <summary>
    /// Manually starts a service that has a Manual startup policy or is in a Stopped/Failed state.
    /// </summary>
    /// <param name="serviceType">The type of the service to start.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task StartServiceAsync(Type serviceType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Manually stops a running service.
    /// </summary>
    /// <param name="serviceType">The type of the service to stop.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task StopServiceAsync(Type serviceType, CancellationToken cancellationToken = default);
}

