using Lifter.Maui;
using Watson.Extensions.Hosting;
using Watson.Extensions.Hosting.Controllers;
using Watson.Extensions.Hosting.Core;
using WatsonWebserver.Lite;

namespace Lifter.Examples.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // 1. Aggiungiamo un HttpClient per poter testare il server dalla nostra UI
        builder.Services.AddHttpClient();

        // 2. Registriamo il Watson Webserver.
        // Questa chiamata registra internamente un IHostedService per gestire il server.
        builder.Services.AddWatsonWebserver<WebserverLite>(options =>
        {
            options.Port = 8080;
            options.Hostname = "localhost";

            // Scansiona l'assembly per trovare classi Controller
            options.MapControllers();

            // Aggiunge un endpoint stile Minimal API
            options.MapGet("/", () => Results.Ok("Welcome from Watson running inside MAUI!"));
        });

        // 3. Attiviamo Lifter.
        // Lifter scoprirà l'IHostedService registrato da Watson e ne gestirà il ciclo di vita
        // in base agli eventi dell'app MAUI.
        builder.SupportHostedServices();

        return builder.Build();
    }
}
