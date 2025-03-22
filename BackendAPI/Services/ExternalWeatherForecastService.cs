using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BackendAPI.Domains;

namespace BackendAPI.Services;

public class ExternalWeatherForecastService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ExternalWeatherForecastService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<WeatherForecast[]> GetWeatherForecastAsync()
    {
        try
        {
            var token = await GetAuth0TokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync($"{_configuration["BackendResourceServiceUrl"]}/resource");
            if (!response.IsSuccessStatusCode) return [];
            var content = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<WeatherForecast[]>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching weather data: {e.Message}");
            return [];
        }
    }
    
    private async Task<string?> GetAuth0TokenAsync()
    {
        var clientId = _configuration["Auth0:ClientId"];
        var clientSecret = _configuration["Auth0:ClientSecret"];
        var audience = _configuration["Auth0:Audience"];
        var tokenUrl = $"{_configuration["Auth0:Domain"]}/oauth/token";

        var requestBody = new
        {
            client_id = clientId,
            client_secret = clientSecret,
            audience = audience,
            grant_type = "client_credentials"
        };

        var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(tokenUrl, requestContent);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

        return tokenResponse.GetProperty("access_token").GetString();
    }
}
