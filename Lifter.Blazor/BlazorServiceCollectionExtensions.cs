using Lifter.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Lifter.Blazor;

public static class BlazorServiceCollectionExtensions
{
    /// <summary>
    /// Registers the Lifter WatchDog and all necessary services
    /// for managing IHostedService instances within a Blazor WASM application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddLifter(this IServiceCollection services)
    {
        // Riutilizza il metodo di estensione già presente in Lifter.Core
        services.AddLifterWatchDog();
        return services;
    }
}