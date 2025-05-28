using SplittLib.Models;

namespace SplittLib.Services;
public class CheckItemService
{
    private readonly HttpClient _httpClient;

    public CheckItemService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5179/api/v1/") // Replace with your API URL
        };
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
