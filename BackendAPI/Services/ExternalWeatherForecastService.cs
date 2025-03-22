using BackendAPI.Domains;

namespace BackendAPI.Services;

using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
}
