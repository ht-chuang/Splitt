using System.Text;
using System.Text.Json;


namespace Splitt.Services;

public class CheckClient
{
    private readonly HttpClient _httpClient;

    public CheckClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5179/api/v1/") // Replace with your API URL
        };
    }

    public async Task<int> CreateCheck(Dictionary<string, object> parameters)
    {
        try
        {
            var json = JsonSerializer.Serialize(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5179/api/v1/Check", content);

            if (!response.IsSuccessStatusCode)
                return -1;

            var responseJson = await response.Content.ReadAsStringAsync();

            using JsonDocument doc = JsonDocument.Parse(responseJson);
            JsonElement root = doc.RootElement;

            int checkId = root.GetProperty("id").GetInt32();

            return checkId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating check: {ex.Message}");
            return -1;
        }

    }
}
