# Lifter: A Toolkit for Modern .NET Development

Welcome to **Lifter**, a curated ecosystem of .NET libraries designed to solve common challenges, enhance productivity, and "lift" the developer experience across the .NET landscape. Our mission is to create robust, well-documented tools that bridge gaps in popular frameworks and feel like a natural extension of the platform.

Lifter is not just a single library, but a suite of focused packages. Each package targets a specific problem, providing an elegant, high-quality solution.

### Our First Mission: `IHostedService` in UI Frameworks

The first packages in the Lifter ecosystem tackle a significant challenge: seamlessly integrating and managing `IHostedService` instances in environments like .NET MAUI. This solves a critical problem, allowing you to run background tasks, local servers, and resilient services within your client applications.

## 🤔 Our Philosophy: Why Lifter Exists

The .NET ecosystem is vast and powerful, but even mature frameworks have gaps or boilerplate-heavy patterns for certain tasks. Lifter's philosophy is to identify these areas and provide clean, modern, and intuitive solutions.

We believe in:

* **Solving One Problem Well**: Each Lifter package is focused on a specific, well-defined problem.
* **Modern .NET Patterns**: We leverage the best of modern .NET, including dependency injection, hosting abstractions, and clean extension methods.
* **Excellent Developer Experience**: Our goal is to create APIs that are easy to use, well-documented, and just *work*.

Our first offering, which brings `IHostedService` support to .NET MAUI, is a perfect example of this philosophy in action. It takes a powerful but inaccessible server-side pattern and makes it a first-class citizen in a client-side framework.

## ✨ Core Features of the Hosting Package

* **Seamless `IHostedService` Integration**: Add long-running background services to your applications with zero boilerplate.
* **Unified Dependency Injection**: Your hosted services share the exact same DI container as your main application, enabling seamless communication and state sharing.
* **Advanced Lifecycle Control (WatchDog)**: Go beyond simple start/stop with a powerful `WatchDog` service that provides:
    * Configurable startup policies (Automatic/Manual).
    * Resilient restart policies (Automatic with attempt limits/Manual).
    * Real-time status monitoring and state-change notifications.
* **Thread-Safe by Design**: The WatchDog service is built with concurrency in mind, ensuring safe management of services even in complex multi-threaded scenarios.
* **Platform Agnostic Core**: `Lifter.Core` contains all the main logic and has no dependencies on any specific UI framework, making it extensible for future integrations (e.g., Avalonia, WPF).
* **Lightweight and Unobtrusive**: Lifter integrates cleanly into your application's startup process without imposing a heavy framework.

## 📦 Packages in the Ecosystem

| Package         | NuGet | Description                                                                                              |
| --------------- | ----- | -------------------------------------------------------------------------------------------------------- |
| **`Lifter.Core`** |       | The core library containing the `HostManager` and the advanced `WatchDog` service. Platform-agnostic. |
| **`Lifter.Maui`** |       | The integration layer for .NET MAUI. It connects the core logic to the MAUI application lifecycle.     |

## 🚀 Getting Started with .NET MAUI

Let's integrate a background service into a .NET MAUI application. This example will run a local web server using `Watson.Extensions.Hosting` that starts and stops with the mobile/desktop app.

### 1. Installation

First, install the necessary packages for your .NET MAUI project.

```shell
dotnet add package Lifter.Maui
dotnet add package Watson.Extensions.Hosting
dotnet add package Watson.Lite
```

### 2. Define Your Services

In your `MauiProgram.cs`, register your services just as you would in any other .NET application. Notice that `Watson.Extensions.Hosting` automatically registers an `IHostedService` for you.

```csharp
// MauiProgram.cs
using Lifter.Maui;
using Watson.Extensions.Hosting;
using WatsonWebserver.Lite;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        // 1. Register your services as usual.
        // This could be any IHostedService. Here, we add a local web server.
        builder.Services.AddWatsonWebserver<WebserverLite>(options =>
        {
            options.Port = 8080;
            options.Hostname = "localhost";
            options.MapGet("/", () => Results.Ok("Hello from MAUI Hosted Service!"));
        });

        // 2. Add Lifter support.
        // This single call wires everything up.
        builder.SupportHostedServices();

        return builder.Build();
    }
}
```

That's it! When your MAUI application starts, `Lifter` will discover the `IHostedService` registered by `AddWatsonWebserver` and automatically call `StartAsync`. When the application is closed, `StopAsync` will be called, gracefully shutting down the web server.

## 📚 Advanced Usage: The WatchDog Service

For scenarios requiring fine-grained control, resilience, and monitoring, `Lifter` provides a powerful `WatchDog` service.

### Why Use the WatchDog?

The default behavior is great for simple fire-and-forget services. The WatchDog is for when you need more control:

* **Manual Start**: Trigger a service based on user input (e.g., user logs in, presses a "Sync" button).
* **Automatic Restarts**: Automatically restart a service if it fails unexpectedly, with configurable retry limits.
* **Status Monitoring**: Query the state of any service (`Stopped`, `Starting`, `Running`, `Failed`) to update your UI.
* **Notifications**: Subscribe to events to react instantly when a service's state changes.

### 1. Enabling the WatchDog

Instead of `AddHostedService`, you register your services and the WatchDog using our dedicated extension method in `MauiProgram.cs`.

```csharp
// MauiProgram.cs
using Lifter.Core; // Namespace for AddLifterWatchDog

builder.Services.AddLifterWatchDog(options =>
{
    // Register your hosted services here.
    options.AddHostedService<MyResilientService>(serviceOptions =>
    {
        // This service will start automatically when the app starts.
        serviceOptions.StartupPolicy = StartupPolicy.Automatic;

        // If it fails, Lifter will try to restart it up to 5 times.
        serviceOptions.RestartPolicy = RestartPolicy.Automatic;
        serviceOptions.MaxRestartAttempts = 5;
    });

    options.AddHostedService<MyManualService>(serviceOptions =>
    {
        // This service will NOT start automatically.
        // We will start it later using the IHostManagerWatchDog interface.
        serviceOptions.StartupPolicy = StartupPolicy.Manual;
        serviceOptions.RestartPolicy = RestartPolicy.Manual;
    });
});

// You still need to call this to hook into the MAUI lifecycle.
builder.SupportHostedServices();
```

### 2. Controlling Services Manually

Inject the `IHostManagerWatchDog` interface into your view models or pages to control your services at runtime.

```csharp
using Lifter.Core.WatchDog;

public class MyViewModel
{
    private readonly IHostManagerWatchDog _watchDog;

    public MyViewModel(IHostManagerWatchDog watchDog)
    {
        _watchDog = watchDog;
    }

    public async Task StartSyncService()
    {
        // Start a service that was registered with a Manual startup policy.
        await _watchDog.StartServiceAsync<MyManualService>();
    }

    public async Task StopSyncService()
    {
        await _watchDog.StopServiceAsync<MyManualService>();
    }

    public HostedServiceState GetSyncServiceState()
    {
        // Get the current state to update the UI.
        return _watchDog.GetServiceState<MyManualService>();
    }
}
```

### 3. Receiving State Notifications

The `WatchDog` exposes an event that fires whenever any service changes its state. This is perfect for updating the UI in real-time.

```csharp
public class MyViewModel : IDisposable
{
    private readonly IHostManagerWatchDog _watchDog;

    public MyViewModel(IHostManagerWatchDog watchDog)
    {
        _watchDog = watchDog;
        _watchDog.OnStateChanged += HandleServiceStateChange;
    }

    private void HandleServiceStateChange(object sender, HostedServiceState state)
    {
        // This event fires on a background thread.
        // Ensure you dispatch UI updates to the main thread.
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (state.ServiceType == typeof(MyManualService))
            {
                Console.WriteLine($"MyManualService new state: {state.Status}");
                // Update UI properties based on state.Status, state.Exception, etc.
            }
        });
    }

    public void Dispose()
    {
        _watchDog.OnStateChanged -= HandleServiceStateChange;
    }
}
```

## 🤝 How to Contribute

We welcome contributions from the community! Whether it's a bug fix, a new feature, or a documentation improvement, your help is greatly appreciated.

1.  **Report Bugs**: If you find an issue, please [open an issue](https://github.com/your-repo/Lifter/issues) and provide detailed steps to reproduce it.
2.  **Suggest Features**: Have a great idea for a new integration (e.g., Avalonia, WPF) or an improvement? [Open an issue](https://github.com/your-repo/Lifter/issues) to start a discussion.
3.  **Submit Pull Requests**: Feel free to fork the repository, create a new branch for your changes, and open a Pull Request.

## ❤️ Support the Project

The easiest way to show your support is by **starring the repository** on GitHub! ⭐

This helps raise the project's visibility and motivates us to keep improving it and expanding the ecosystem.

## 📜 License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).
```
