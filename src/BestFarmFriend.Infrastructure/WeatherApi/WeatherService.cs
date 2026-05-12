using System.Net.Http.Json;
using BestFarmFriend.Core.Interfaces;
using BestFarmFriend.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BestFarmFriend.Infrastructure.WeatherApi;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly IMemoryCache _cache;
    private readonly ILogger<WeatherService> _logger;

    private const string BaseUrl = "https://api.open-meteo.com/v1/forecast";

    public WeatherService(HttpClient http, IMemoryCache cache, ILogger<WeatherService> logger)
    {
        _http = http;
        _cache = cache;
        _logger = logger;
    }

    public async Task<WeatherSnapshot?> GetCurrentWeatherAsync(Location location, CancellationToken ct = default)
    {
        var snapshot = await GetCurrentWeatherAsync(location.Latitude, location.Longitude, ct);
        if (snapshot != null && string.IsNullOrEmpty(snapshot.LocationTimeZone))
            snapshot.LocationTimeZone = location.Timezone;
        return snapshot;
    }

    public async Task<WeatherSnapshot?> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        string cacheKey = $"weather_{latitude:F4}_{longitude:F4}";

        if (_cache.TryGetValue(cacheKey, out WeatherSnapshot? cached))
            return cached;

        string url = BuildUrl(latitude, longitude);

        try
        {
            var response = await _http.GetFromJsonAsync<OpenMeteoCurrentResponse>(url, ct);
            if (response == null) return null;

            var snapshot = WeatherMapper.Map(response, latitude, longitude);
            _cache.Set(cacheKey, snapshot, TimeSpan.FromMinutes(15));
            return snapshot;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for {Lat},{Lon}", latitude, longitude);
            return null;
        }
    }

    private static string BuildUrl(double lat, double lon) =>
        $"{BaseUrl}?latitude={lat}&longitude={lon}" +
        "&current=temperature_2m,apparent_temperature,relative_humidity_2m,dew_point_2m," +
        "precipitation,weather_code,wind_speed_10m,wind_direction_10m,wind_gusts_10m," +
        "surface_pressure,visibility,uv_index" +
        "&hourly=temperature_2m,precipitation_probability,precipitation,wind_speed_10m,wind_gusts_10m,weather_code" +
        "&daily=temperature_2m_max,temperature_2m_min,precipitation_sum,precipitation_probability_max," +
        "wind_speed_10m_max,weather_code,sunrise,sunset" +
        "&wind_speed_unit=mph&temperature_unit=fahrenheit&precipitation_unit=inch" +
        "&forecast_days=7&timezone=auto";
}
