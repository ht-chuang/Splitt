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

    public static CheckItem CreateCheckItem(string name, string description, int quantity, decimal unitPrice, decimal totalPrice, int checkId)
    {
        return new CheckItem
        {
            Name = name,
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice,
            TotalPrice = totalPrice,
            CheckId = checkId
        };
    }

}
