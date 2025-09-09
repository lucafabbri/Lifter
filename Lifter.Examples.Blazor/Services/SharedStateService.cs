namespace Lifter.Examples.Blazor.Services;

/// <summary>
/// A singleton service to hold the application state that can be updated
/// by background services and consumed by UI components.
/// </summary>
public class SharedStateService
{
    /// <summary>
    /// The message that will be displayed in the UI.
    /// </summary>
    public string CurrentMessage { get; private set; } = "Waiting for the first update from the background service...";

    /// <summary>
    /// Event that is triggered whenever the state changes.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// Updates the current message and notifies listeners.
    /// </summary>
    /// <param name="message">The new message.</param>
    public void UpdateMessage(string message)
    {
        CurrentMessage = message;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
