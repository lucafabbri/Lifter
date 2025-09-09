# Lifter: A Toolkit for Modern .NET Development

Welcome to **Lifter**, a curated ecosystem of .NET libraries designed to solve common challenges, enhance productivity, and "lift" the developer experience across the .NET landscape. Our mission is to create robust, well-documented tools that bridge gaps in popular frameworks and feel like a natural extension of the platform.

Lifter is not just a single library, but a suite of focused packages. Each package targets a specific problem, providing an elegant, high-quality solution.

### Our First Mission: `IHostedService` in UI Frameworks

The first packages in the Lifter ecosystem tackle a significant challenge: seamlessly integrating and managing `IHostedService` instances in environments like **.NET MAUI** and **Blazor WebAssembly**. This solves a critical problem, allowing you to run background tasks, local servers, and resilient services within your client applications.

## 🤔 Our Philosophy: Why Lifter Exists

The .NET ecosystem is vast and powerful, but even mature frameworks have gaps or boilerplate-heavy patterns for certain tasks. Lifter's philosophy is to identify these areas and provide clean, modern, and intuitive solutions.

We believe in:

* **Solving One Problem Well**: Each Lifter package is focused on a specific, well-defined problem.
* **Modern .NET Patterns**: We leverage the best of modern .NET, including dependency injection, hosting abstractions, and clean extension methods.
* **Excellent Developer Experience**: Our goal is to create APIs that are easy to use, well-documented, and just *work*.

Our first offering, which brings `IHostedService` support to modern UI frameworks, is a perfect example of this philosophy in action. It takes a powerful but inaccessible server-side pattern and makes it a first-class citizen in client-side applications.

## ✨ Core Features of the Hosting Package

* **Seamless `IHostedService` Integration**: Add long-running background services to your applications with zero boilerplate.
* **Unified Dependency Injection**: Your hosted services share the exact same DI container as your main application, enabling seamless communication and state sharing.
* **Advanced Lifecycle Control (WatchDog)**: Go beyond simple start/stop with a powerful `WatchDog` service that provides:
  * Configurable startup policies (Automatic/Manual).
  * Resilient restart policies (Automatic with attempt limits/Manual).
  * Real-time status monitoring and state-change notifications.
* **Thread-Safe by Design**: The WatchDog service is built with concurrency in mind, ensuring safe management of services even in complex multi-threaded scenarios.
* **Platform Agnostic Core**: `Lifter.Core` contains all the main logic and has no dependencies on any specific UI framework, making it extensible for future integrations.
* **Lightweight and Unobtrusive**: Lifter integrates cleanly into your application's startup process without imposing a heavy framework.

## 📦 Packages in the Ecosystem

| Package             | NuGet | Description                                                                                                 |
| :------------------ | :---- | :---------------------------------------------------------------------------------------------------------- |
| **`Lifter.Core`**   |       | The core library containing the `HostManager` and the advanced `WatchDog` service. Platform-agnostic.       |
| **`Lifter.Maui`**   |       | The integration layer for .NET MAUI. It connects the core logic to the MAUI application lifecycle.          |
| **`Lifter.Blazor`** |       | The integration layer for Blazor WebAssembly. It connects the core logic to the Blazor component lifecycle. |

## 🚀 Getting Started with .NET MAUI

Let's integrate a background service into a .NET MAUI application. This example will run a local web server using `Watson.Extensions.Hosting` that starts and stops with the mobile/desktop app.

### 1. Installation

First, install the necessary packages for your .NET MAUI project.

```shell
dotnet add package Lifter.Maui
dotnet add package Watson.Extensions.Hosting
```

### 2. Define Your Services

In your `MauiProgram.cs`, register your services just as you would in any other .NET application.

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
        });

        // 2. Add Lifter support.
        // This single call wires everything up.
        builder.SupportHostedServices();

        return builder.Build();
    }
}
```

That's it! When your MAUI application starts, `Lifter` will discover the `IHostedService` and automatically call `StartAsync`. When the application is closed, `StopAsync` will be called.

## 🚀 Getting Started with Blazor WebAssembly

Let's integrate a periodic data-polling service into a Blazor WASM application.

### 1. Installation

First, install the necessary package for your Blazor WASM project.

```shell
dotnet add package Lifter.Blazor
```

### 2. Define Your Service

Create your background service by inheriting from `BackgroundService`.

```csharp
// Services/DataPollingService.cs
using Microsoft.Extensions.Hosting;

public class DataPollingService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Polling for new data...");
            await timer.WaitForNextTick(stoppingToken);
        }
    }
}
```

### 3. Configure Services and Add the Host Component

In `Program.cs`, register Lifter and your service. Then, add the `<LifterHost />` component to your `MainLayout.razor` file.

```csharp
// Program.cs
using Lifter.Blazor;
using Lifter.Core; // For AddHostedServiceWithPolicies
using YourApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// 1. Add Lifter support
builder.Services.AddLifter();

// 2. Register your services with desired policies
builder.Services.AddHostedServiceWithPolicies<DataPollingService>(options =>
{
    options.Startup = Lifter.Core.WatchDog.StartupPolicy.Automatic;
});

await builder.Build().RunAsync();
```

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase
@using Lifter.Blazor

@* ... your existing layout markup ... *@

@* 3. Add this invisible component to manage the lifecycle *@
<LifterHost />
```

That's it! When your Blazor application loads, the `LifterHost` component will start all automatic services.

## 📚 Advanced Usage: The WatchDog Service

For scenarios requiring fine-grained control, resilience, and monitoring, `Lifter` provides a powerful `WatchDog` service, available in both MAUI and Blazor.

### Why Use the WatchDog?

* **Manual Start**: Trigger a service based on user input.
* **Automatic Restarts**: Automatically restart a service if it fails unexpectedly.
* **Status Monitoring**: Query the state of any service (`Stopped`, `Running`, `Failed`) to update your UI.
* **Notifications**: Subscribe to events to react instantly when a service's state changes.

### 1. Enabling the WatchDog

In `Program.cs` (or `MauiProgram.cs`), register the WatchDog and your services with policies.

```csharp
// Program.cs or MauiProgram.cs
using Lifter.Core;
using Lifter.Core.WatchDog;

// 1. Add the WatchDog service
builder.Services.AddLifterWatchDog();

// 2. Register your hosted services with policies
builder.Services.AddHostedServiceWithPolicies<MyResilientService>(options =>
{
    options.Startup = StartupPolicy.Automatic;
    options.Restart = RestartPolicy.OnFailure;
    options.MaxRestartAttempts = 5;
});

builder.Services.AddHostedServiceWithPolicies<MyManualService>(options =>
{
    options.Startup = StartupPolicy.Manual;
});


// 3. For MAUI, you still need to call this to hook into the lifecycle.
// For Blazor, the <LifterHost /> component handles this.
#if MAUI
builder.SupportHostedServices();
#endif
```

### 2. Controlling Services Manually

Inject the `IHostManagerWatchDog` interface into your pages, components, or view models to control your services at runtime. The C# code is the same for both Blazor and .NET MAUI.

```csharp
using Lifter.Core.WatchDog;

public class MyViewModel // Or a @code block in a Razor component
{
    private readonly IHostManagerWatchDog _watchDog;

    public MyViewModel(IHostManagerWatchDog watchDog)
    {
        _watchDog = watchDog;
    }

    public async Task StartSyncService()
    {
        await _watchDog.StartServiceAsync(typeof(MyManualService));
    }

    public HostedServiceState GetSyncServiceState()
    {
        return _watchDog.GetStatus(typeof(MyManualService));
    }
}
```

### 3. Receiving State Notifications

The `WatchDog` exposes a `StatusChanged` event that fires whenever any service changes its state. This is perfect for updating the UI in real-time, but remember to dispatch UI updates to the correct thread.

**For .NET MAUI:**

```csharp
// In a ViewModel or Page code-behind
_watchDog.StatusChanged += (state) =>
{
    // Ensure you dispatch UI updates to the main thread.
    MainThread.BeginInvokeOnMainThread(() =>
    {
        if (state.Instance.GetType() == typeof(MyManualService))
        {
            Console.WriteLine($"MyManualService new state: {state.Status}");
        }
    });
};
```

**For Blazor WebAssembly:**

```razor
@* In a Razor component *@
@implements IDisposable
@inject IHostManagerWatchDog WatchDog

@code {
    protected override void OnInitialized()
    {
        WatchDog.StatusChanged += HandleServiceStateChange;
    }

    private void HandleServiceStateChange(HostedServiceState state)
    {
        // Use InvokeAsync to safely update the UI from the event handler.
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        WatchDog.StatusChanged -= HandleServiceStateChange;
    }
}
```

## 🤝 How to Contribute

We welcome contributions from the community! Whether it's a bug fix, a new feature, or a documentation improvement, your help is greatly appreciated.

1. **Report Bugs**: If you find an issue, please [open an issue](https://github.com/your-repo/Lifter/issues).
2. **Suggest Features**: Have a great idea for a new integration? [Open an issue](https://github.com/your-repo/Lifter/issues) to start a discussion.
3. **Submit Pull Requests**: Feel free to fork the repository and open a Pull Request.

## ❤️ Support the Project

The easiest way to show your support is by **starring the repository** on GitHub! ⭐

This helps raise the project's visibility and motivates us to keep improving it and expanding the ecosystem.

## 📜 License

This project is licensed under the [MIT License](https://opensource.org/licenses/MIT).