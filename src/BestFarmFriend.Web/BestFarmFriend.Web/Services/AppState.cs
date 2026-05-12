using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Web.Services;

public class AppState
{
    private Location? _currentLocation;
    private UserSettings _settings = new();

    public Location? CurrentLocation
    {
        get => _currentLocation;
        set { _currentLocation = value; OnLocationChanged?.Invoke(); }
    }

    public UserSettings Settings
    {
        get => _settings;
        set { _settings = value; OnSettingsChanged?.Invoke(); }
    }

    public event Action? OnLocationChanged;
    public event Action? OnSettingsChanged;

    public string WindDirectionLabel(double degrees)
    {
        string[] dirs = ["N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE",
                         "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW"];
        int idx = (int)((degrees + 11.25) / 22.5) % 16;
        return dirs[idx];
    }

    public string ConditionDisplayName(WeatherCondition condition) => condition switch
    {
        WeatherCondition.ClearSky => "Clear Sky",
        WeatherCondition.MainlyClear => "Mainly Clear",
        WeatherCondition.PartlyCloudy => "Partly Cloudy",
        WeatherCondition.Overcast => "Overcast",
        WeatherCondition.Fog => "Foggy",
        WeatherCondition.Drizzle => "Drizzle",
        WeatherCondition.Rain => "Rain",
        WeatherCondition.HeavyRain => "Heavy Rain",
        WeatherCondition.Thunderstorm => "Thunderstorm",
        WeatherCondition.Snow => "Snow",
        WeatherCondition.Sleet => "Sleet",
        WeatherCondition.Windy => "Windy",
        _ => condition.ToString()
    };
}
