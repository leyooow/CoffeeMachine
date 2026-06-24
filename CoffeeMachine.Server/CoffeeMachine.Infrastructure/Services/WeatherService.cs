using System.Net.Http.Json;
using CoffeeMachine.Application.Interface.Services;

namespace CoffeeMachine.Application.Services;

public sealed class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<double> GetCurrentTemperatureAsync()
    {
        // My current location longitude & latitude
        var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(
            "https://api.open-meteo.com/v1/forecast?latitude=14.6&longitude=120.97&current_weather=true");


        // Uncomment for iced coffee
        //response.Current_Weather.Temperature = 35;

        return response?.Current_Weather?.Temperature ?? 25;
    }

    private class WeatherResponse
    {
        public CurrentWeatherData? Current_Weather { get; set; }

        public class CurrentWeatherData
        {
            public double Temperature { get; set; }
        }
    }
}
