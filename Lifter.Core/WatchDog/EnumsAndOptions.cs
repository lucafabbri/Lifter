namespace Lifter.Core.WatchDog;

/// <summary>
/// Defines the startup behavior for a hosted service.
/// </summary>
public enum StartupPolicy
{
    /// <summary>
    /// The service starts automatically with the application.
    /// </summary>
    Automatic,
    /// <summary>
    /// The service must be started manually via the WatchDog.
    /// </summary>
    Manual
}

/// <summary>
/// Defines the restart behavior for a hosted service upon failure.
/// </summary>
public enum RestartPolicy
{
    /// <summary>
    /// The service will not be restarted automatically on failure.
    /// </summary>
    Manual,
    /// <summary>
    /// The service will be restarted automatically if it fails.
    /// </summary>
    OnFailure
}

/// <summary>
/// Configuration options for a hosted service managed by the WatchDog.
/// </summary>
public class HostedServiceOptions
{
    /// <summary>
    /// Gets or sets the startup policy. Defaults to Automatic.
    /// </summary>
    public StartupPolicy Startup { get; set; } = StartupPolicy.Automatic;

    /// <summary>
    /// Gets or sets the restart policy. Defaults to Manual.
    /// </summary>
    public RestartPolicy Restart { get; set; } = RestartPolicy.Manual;

    /// <summary>
    /// Gets or sets the maximum number of restart attempts for a failed service.
    /// Only applicable if Restart policy is OnFailure. Defaults to 5.
    /// </summary>
    public int MaxRestartAttempts { get; set; } = 5;

    /// <summary>
    /// Gets or sets the delay before attempting a restart. Defaults to 5 seconds.
    /// </summary>
    public TimeSpan RestartDelay { get; set; } = TimeSpan.FromSeconds(5);
}

