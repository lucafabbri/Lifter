using Watson.Extensions.Hosting.Controllers;
using Watson.Extensions.Hosting.Core;

namespace Lifter.Examples.Maui;

/// <summary>
/// Un controller in stile ASP.NET Core che verrà ospitato dal server Watson
/// in esecuzione all'interno della nostra app MAUI.
/// </summary>
[Route("api/greeting")]
public class GreetingController : ControllerBase
{
    // Gestisce le richieste GET a /api/greeting/{name}
    [HttpGet("{name}")]
    public IActionResult SayHello([FromRoute] string name)
    {
        return Results.Ok(new { message = $"Hello, {name}! The time is {DateTime.Now:T}" });
    }
}