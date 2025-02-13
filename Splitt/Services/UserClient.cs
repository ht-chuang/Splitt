using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Splitt.Services;
public class UserClient
{
    private readonly HttpClient _httpClient;

    public UserClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5179/api/v1/") // Replace with your API URL
        };
    }

    public async Task<HttpResponseMessage> GetUser()
    {
        var response = await _httpClient.GetAsync("User/1");
        return response;
    }

    public async Task<HttpResponseMessage> GetUser(int id)
    {
        var response = await _httpClient.GetAsync($"User/{id}");
        return response;
    }
}
