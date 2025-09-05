namespace Lifter.Examples.Maui;

public partial class MainPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public MainPage()
    {
        InitializeComponent();
        // In un'app reale, l'HttpClient verrebbe iniettato.
        // Per semplicità, lo creiamo qui.
        _httpClient = new HttpClient();
    }

    private async void OnTestServerClicked(object sender, EventArgs e)
    {
        ServerResponseEditor.Text = "Sending request...";
        try
        {
            // Chiamiamo l'endpoint del nostro GreetingController
            var response = await _httpClient.GetAsync("http://localhost:8080/api/greeting/MAUI");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                ServerResponseEditor.Text = content;
            }
            else
            {
                ServerResponseEditor.Text = $"Error: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            ServerResponseEditor.Text = $"Failed to connect to the server. Is it running?\n\nError: {ex.Message}";
        }
    }
}

