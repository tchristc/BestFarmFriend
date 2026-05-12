namespace BigBestFarm.Core.Models;

public class WeatherSnapshot
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public DateTime ObservedAt { get; set; }

    // Temperature
    public double TemperatureF { get; set; }
    public double FeelsLikeF { get; set; }
    public double DewPointF { get; set; }
    public double DailyHighF { get; set; }
    public double DailyLowF { get; set; }

    // Humidity
    public double HumidityPct { get; set; }

    // Wind
    public double WindSpeedMph { get; set; }
    public double WindGustMph { get; set; }
    public double WindDirectionDeg { get; set; }

    // Precipitation
    public double PrecipitationRateInPerHr { get; set; }
    public double PrecipPast1hIn { get; set; }
    public double PrecipPast24hIn { get; set; }

    // Pressure
    public double PressureHpa { get; set; }
    public PressureTrend PressureTrend { get; set; }

    // Other
    public double UvIndex { get; set; }
    public double VisibilityMiles { get; set; }
    public WeatherCondition Condition { get; set; }

    // Sun
    public DateTime SunriseUtc { get; set; }
    public DateTime SunsetUtc { get; set; }

    // Forecast
    public List<WeatherForecastHour> HourlyForecast { get; set; } = new();
    public List<WeatherForecastDay> DailyForecast { get; set; } = new();
}

public enum PressureTrend { Falling, Steady, Rising }

public enum WeatherCondition
{
    ClearSky,
    MainlyClear,
    PartlyCloudy,
    Overcast,
    Fog,
    Drizzle,
    Rain,
    HeavyRain,
    Thunderstorm,
    Snow,
    Sleet,
    Windy
}
