---
layout: default
title: Home
nav_order: 1
description: "Lifter - A toolkit for modern .NET development"
permalink: /
---

# Lifter
{: .fs-9 }

A toolkit for modern .NET development that lifts your development experience
{: .fs-6 .fw-300 }

[Get Started](/Lifter/latest/getting-started){: .btn .btn-primary .fs-5 .mb-4 .mb-md-0 .mr-2 }
[View on GitHub](https://github.com/lucafabbri/Lifter){: .btn .fs-5 .mb-4 .mb-md-0 }

---

## Welcome to Lifter

**Lifter** is a curated ecosystem of .NET libraries designed to solve common challenges and enhance productivity across .NET platforms. Our mission is to create robust, well-documented tools that feel like a natural extension of the platform.

### 🎯 Our First Mission: IHostedService Everywhere

The Lifter packages bring **`IHostedService`** support to UI frameworks where it's not natively available:

- ✅ **.NET MAUI** - Mobile and desktop apps
- ✅ **Avalonia UI** - Cross-platform desktop apps
- ✅ **Blazor WebAssembly** - Browser applications

Run background tasks, local servers, and resilient services within your client applications with the same patterns you use on the server.

---

## 📦 Available Packages

| Package | Version | Description |
|:--------|:--------|:------------|
| [**Lifter.Core**](/Lifter/latest/packages/core) | [![NuGet](https://img.shields.io/nuget/v/Lifter.Core.svg)](https://www.nuget.org/packages/Lifter.Core/) | Core library with HostManager and WatchDog service |
| [**Lifter.Maui**](/Lifter/latest/packages/maui) | [![NuGet](https://img.shields.io/nuget/v/Lifter.Maui.svg)](https://www.nuget.org/packages/Lifter.Maui/) | .NET MAUI integration for mobile and desktop |
| [**Lifter.Avalonia**](/Lifter/latest/packages/avalonia) ⭐ NEW | [![NuGet](https://img.shields.io/nuget/v/Lifter.Avalonia.svg)](https://www.nuget.org/packages/Lifter.Avalonia/) | Avalonia UI with HostedApplication support |
| [**Lifter.Blazor**](/Lifter/latest/packages/blazor) | [![NuGet](https://img.shields.io/nuget/v/Lifter.Blazor.svg)](https://www.nuget.org/packages/Lifter.Blazor/) | Blazor WebAssembly integration |

---

## ✨ Key Features

- **🚀 Zero Boilerplate** - Add background services with a single line of code
- **🔧 Unified DI** - Same dependency injection container across your entire app
- **⚡ WatchDog Service** - Advanced lifecycle control with restart policies and monitoring
- **🎯 Thread-Safe** - Built for concurrent environments
- **📱 Cross-Platform** - Works on Windows, macOS, Linux, iOS, Android, and browser

---

## Quick Example

### .NET MAUI
```csharp
// MauiProgram.cs
using Lifter.Maui;

builder.Services.AddHostedService<MyBackgroundService>();
builder.SupportHostedServices(); // ✨ That's it!
```

### Avalonia UI
```csharp
// App.cs
using Lifter.Avalonia;

public class App : HostedApplication<MainView>
{
    protected override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<MyBackgroundService>();
    }
}
```

### Blazor WebAssembly
```razor
@* MainLayout.razor *@
<LifterHost />
```

---

## 📚 Documentation Versions

- [**Latest (v1.1)**](/Lifter/latest/) - Current stable version
- [v1.0](/Lifter/v1.0/) - Previous version

---

## 🤝 Community

- ⭐ [Star us on GitHub](https://github.com/lucafabbri/Lifter)
- 🐛 [Report Issues](https://github.com/lucafabbri/Lifter/issues)
- 💡 [Request Features](https://github.com/lucafabbri/Lifter/issues/new)
- 📖 [Read the Docs](/Lifter/latest/getting-started)

---

## 📜 License

Lifter is licensed under the [MIT License](https://opensource.org/licenses/MIT).
