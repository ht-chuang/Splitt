using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SplittLib.Models;

namespace Splitt.Services;

public class CheckItemClient
{
    private readonly HttpClient _httpClient;

    public CheckItemClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5179/api/v1/") // Replace with your API URL
        };
    }

    public async Task<List<CheckItem>> GetCheckItems()
    {
        var response = await _httpClient.GetAsync("CheckItem");
        string json = await response.Content.ReadAsStringAsync();
        var checkItemList = JsonSerializer.Deserialize<List<CheckItem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return checkItemList ?? new List<CheckItem>();
    }

    public async Task<HttpResponseMessage> GetCheckItem(int id)
    {
        var response = await _httpClient.GetAsync($"CheckItem/{id}");
        return response;
    }

    public async Task<CheckItem?> CreateCheckItem(Dictionary<string, object> parameters)
    {
        try
        {
            var json = JsonSerializer.Serialize(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5179/api/v1/CheckItem", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonString = await response.Content.ReadAsStringAsync();

            var checkItem = JsonSerializer.Deserialize<CheckItem>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return checkItem;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating check: {ex.Message}");
            return null;
        }

    }

    public async Task<int> DeleteCheckItem(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"http://localhost:5179/api/v1/CheckItem/{id}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error deleting check item with ID {id}: {response.ReasonPhrase}");
                return -1;
            }
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting check item: {ex.Message}");
            return -1;
        }
    }

}
