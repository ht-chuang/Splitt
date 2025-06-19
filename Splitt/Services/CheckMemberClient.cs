using System.Text;
using System.Text.Json;
using SplittLib.Models;

namespace Splitt.Services;

public class CheckMemberClient
{
    private readonly HttpClient _httpClient;

    public CheckMemberClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5179/api/v1/") // Replace with your API URL
        };
    }

    public async Task<CheckMember?> CreateCheckMember(Dictionary<string, object?> parameters)
    {
        try
        {
            var json = JsonSerializer.Serialize(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:5179/api/v1/CheckMember", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonString = await response.Content.ReadAsStringAsync();

            var checkMember = JsonSerializer.Deserialize<CheckMember>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return checkMember;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating check: {ex.Message}");
            return null;
        }

    }

    public async Task<List<CheckMember>?> UpdateCheckMembers(List<Dictionary<string, object>> parameters)
    {
        try
        {
            var json = JsonSerializer.Serialize(parameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync("http://localhost:5179/api/v1/CheckMember/bulk", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonString = await response.Content.ReadAsStringAsync();

            var checkMembers = JsonSerializer.Deserialize<List<CheckMember>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return checkMembers;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating check: {ex.Message}");
            return null;
        }
    }

    public async Task<int> DeleteCheckMember(int id)
    {
        var response = await _httpClient.DeleteAsync($"CheckMember/{id}");
        if (response.IsSuccessStatusCode)
        {
            return id;
        }
        else
        {
            return -1; // Indicate failure
        }
    }
}
