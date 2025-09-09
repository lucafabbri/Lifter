using Microsoft.Extensions.Hosting;

namespace Lifter.Examples.Blazor.Services;

/// <summary>
/// A background service that periodically updates a shared state with the current time.
/// </summary>
public class TimeUpdateService : BackgroundService
{
    private readonly SharedStateService _sharedState;

    public TimeUpdateService(SharedStateService sharedState)
    {
        _sharedState = sharedState;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = $"Last update from Lifter service: {DateTime.Now:HH:mm:ss}";
                _sharedState.UpdateMessage(message);

                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Normal behavior on shutdown
                break;
            }
        }
    }
}
