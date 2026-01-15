using Avalonia.Controls;

namespace Lifter.Avalonia;

/// <summary>
/// Configuration settings for the main desktop window.
/// </summary>
public class WindowConfiguration
{
    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    public string Title { get; set; } = "Application";

    /// <summary>
    /// Gets or sets the window width in device-independent pixels.
    /// </summary>
    public double Width { get; set; } = 1200;

    /// <summary>
    /// Gets or sets the window height in device-independent pixels.
    /// </summary>
    public double Height { get; set; } = 960;

    /// <summary>
    /// Gets or sets the initial window state.
    /// </summary>
    public WindowState WindowState { get; set; } = WindowState.Maximized;

    /// <summary>
    /// Gets or sets the window startup location.
    /// </summary>
    public WindowStartupLocation StartupLocation { get; set; } = WindowStartupLocation.CenterScreen;
}
