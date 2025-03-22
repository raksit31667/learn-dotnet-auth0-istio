using BackendAPI.Domains;
using BackendAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;

[ApiController]
public class WeatherForecastController : ControllerBase
{
    private readonly ExternalWeatherForecastService _externalWeatherForecastService;

    public WeatherForecastController(ExternalWeatherForecastService externalWeatherForecastService)
    {
        _externalWeatherForecastService = externalWeatherForecastService;
    }

    [Route("/weatherforecast")]
    [HttpGet]
    public IActionResult GetWeatherForecast()
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        var forecast =  Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateTime.Now.AddDays(index),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return Ok(forecast);
    }

    [Route("/external-weatherforecast")]
    [HttpGet]
    public IActionResult GetExternalWeatherForecast()
    {
        var forecast = _externalWeatherForecastService.GetWeatherForecastAsync();
        return Ok(forecast.Result);
    }
}
