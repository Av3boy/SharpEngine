using SharpEngine.Rest.Urls;
using System.Diagnostics;
using System.Text.Json;

namespace SharpEngine.Rest.Clients;

public abstract class RestClient
{
    private readonly HttpClient _httpClient;
    public RestClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(AssetStore.BaseUrl);
    }

    public async Task<T> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<T>(json);
        Debug.Assert(result is not null, "Deserialized result is null.");

        return result;
    }
}
