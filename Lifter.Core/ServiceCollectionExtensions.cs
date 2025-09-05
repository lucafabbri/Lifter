using Lifter.Core.WatchDog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Lifter.Core;

// Internal helpers for DI to associate options with a specific service type.
internal abstract class ServiceOptionsProvider
{
    public abstract Type ServiceType { get; }
    public abstract HostedServiceOptions Options { get; }
}

internal class ServiceOptionsProvider<TService> : ServiceOptionsProvider where TService : IHostedService
{
    public override Type ServiceType => typeof(TService);
    public override HostedServiceOptions Options { get; }
    public ServiceOptionsProvider(HostedServiceOptions options) => Options = options;
}

public static class LifterServiceExtensions
{
    /// <summary>
    /// Registers the WatchDog service to enable advanced monitoring and management of hosted services.
    /// </summary>
    public static IServiceCollection AddLifterWatchDog(this IServiceCollection services)
    {
        services.TryAddSingleton<WatchDogService>();
        services.TryAddSingleton<IHostManagerWatchDog>(sp => sp.GetRequiredService<WatchDogService>());
        return services;
    }

    /// <summary>
    /// Registers a hosted service with specific lifecycle policies for the WatchDog to manage.
    /// </summary>
    /// <typeparam name="TService">The type of the hosted service.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">An action to configure the service's policies.</param>
    public static IServiceCollection AddHostedServiceWithPolicies<TService>(this IServiceCollection services, Action<HostedServiceOptions> configureOptions)
        where TService : class, IHostedService
    {
        services.AddSingleton<IHostedService, TService>();

        var options = new HostedServiceOptions();
        configureOptions?.Invoke(options);
        services.AddSingleton(new ServiceOptionsProvider<TService>(options));

        return services;
    }
}

