using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Interfaces;

public interface IWeatherService
{
    Task<WeatherSnapshot?> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken ct = default);
    Task<WeatherSnapshot?> GetCurrentWeatherAsync(Location location, CancellationToken ct = default);
}
