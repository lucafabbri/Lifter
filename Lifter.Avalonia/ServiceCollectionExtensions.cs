using Microsoft.Extensions.DependencyInjection;

namespace Lifter.Avalonia;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to configure Avalonia-specific services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures desktop window settings for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">An action to configure the window settings.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.ConfigureWindow(config =>
    /// {
    ///     config.Title = "My Application";
    ///     config.Width = 1024;
    ///     config.Height = 768;
    ///     config.WindowState = WindowState.Normal;
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection ConfigureWindow(
        this IServiceCollection services,
        Action<WindowConfiguration> configure)
    {
        var config = new WindowConfiguration();
        configure(config);
        services.AddSingleton(config);
        return services;
    }
}
