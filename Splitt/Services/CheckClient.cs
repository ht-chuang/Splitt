using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

    public async Task<HttpResponseMessage> CreateCheck(Dictionary<string, object> parameters)
    {
        var json = JsonSerializer.Serialize(parameters);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:5179/api/v1/Check", content);
        return response;
    }
}
