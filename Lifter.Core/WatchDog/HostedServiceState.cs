using Microsoft.Extensions.Hosting;

namespace Lifter.Core.WatchDog;

/// <summary>
/// Represents the current operational status of a hosted service.
/// </summary>
public enum ServiceStatus
{
    Stopped,
    Starting,
    Running,
    Stopping,
    Failed,
    Restarting
}

/// <summary>
/// A thread-safe container for the complete runtime state of a monitored hosted service.
/// </summary>
public sealed class HostedServiceState
{
    private readonly object _lock = new();

    /// <summary>
    /// The instance of the hosted service.
    /// </summary>
    public IHostedService Instance { get; }

    /// <summary>
    /// The configuration options applied to this service.
    /// </summary>
    public HostedServiceOptions Options { get; }

    /// <summary>
    /// The current status of the service.
    /// </summary>
    public ServiceStatus Status { get; private set; }

    /// <summary>
    /// The last exception that occurred, if the service failed.
    /// </summary>
    public Exception? LastException { get; private set; }

    /// <summary>
    /// The number of restart attempts made since the last successful run.
    /// </summary>
    public int CurrentRestartAttempts { get; private set; }

    /// <summary>
    /// The timestamp of the last status change in UTC.
    /// </summary>
    public DateTime LastStatusChangeUtc { get; private set; }

    internal HostedServiceState(IHostedService instance, HostedServiceOptions options)
    {
        Instance = instance;
        Options = options;
        Status = ServiceStatus.Stopped;
        LastStatusChangeUtc = DateTime.UtcNow;
    }

    internal void TransitionTo(ServiceStatus newStatus, Exception? exception = null)
    {
        lock (_lock)
        {
            Status = newStatus;
            LastStatusChangeUtc = DateTime.UtcNow;

            if (exception != null)
            {
                LastException = exception;
            }

            if (newStatus == ServiceStatus.Restarting)
            {
                CurrentRestartAttempts++;
            }
            else if (newStatus == ServiceStatus.Running || newStatus == ServiceStatus.Stopped)
            {
                CurrentRestartAttempts = 0;
                LastException = null;
            }
        }
    }
}

