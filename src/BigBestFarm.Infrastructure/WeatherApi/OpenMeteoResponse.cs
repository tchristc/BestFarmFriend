using System.Text.Json.Serialization;

namespace BigBestFarm.Infrastructure.WeatherApi;

public class OpenMeteoCurrentResponse
{
    [JsonPropertyName("current")]
    public OpenMeteoCurrent? Current { get; set; }

    [JsonPropertyName("hourly")]
    public OpenMeteoHourly? Hourly { get; set; }

    [JsonPropertyName("daily")]
    public OpenMeteoDaily? Daily { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}

public class OpenMeteoCurrent
{
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double Temperature2m { get; set; }

    [JsonPropertyName("apparent_temperature")]
    public double ApparentTemperature { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public double RelativeHumidity2m { get; set; }

    [JsonPropertyName("dew_point_2m")]
    public double DewPoint2m { get; set; }

    [JsonPropertyName("precipitation")]
    public double Precipitation { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed10m { get; set; }

    [JsonPropertyName("wind_direction_10m")]
    public double WindDirection10m { get; set; }

    [JsonPropertyName("wind_gusts_10m")]
    public double WindGusts10m { get; set; }

    [JsonPropertyName("surface_pressure")]
    public double SurfacePressure { get; set; }

    [JsonPropertyName("visibility")]
    public double Visibility { get; set; }

    [JsonPropertyName("uv_index")]
    public double UvIndex { get; set; }
}

public class OpenMeteoHourly
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("temperature_2m")]
    public List<double> Temperature2m { get; set; } = new();

    [JsonPropertyName("precipitation_probability")]
    public List<double> PrecipitationProbability { get; set; } = new();

    [JsonPropertyName("precipitation")]
    public List<double> Precipitation { get; set; } = new();

    [JsonPropertyName("wind_speed_10m")]
    public List<double> WindSpeed10m { get; set; } = new();

    [JsonPropertyName("wind_gusts_10m")]
    public List<double> WindGusts10m { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();
}

public class OpenMeteoDaily
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("temperature_2m_max")]
    public List<double> TemperatureMax { get; set; } = new();

    [JsonPropertyName("temperature_2m_min")]
    public List<double> TemperatureMin { get; set; } = new();

    [JsonPropertyName("precipitation_sum")]
    public List<double> PrecipitationSum { get; set; } = new();

    [JsonPropertyName("precipitation_probability_max")]
    public List<double> PrecipitationProbabilityMax { get; set; } = new();

    [JsonPropertyName("wind_speed_10m_max")]
    public List<double> WindSpeedMax { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();

    [JsonPropertyName("sunrise")]
    public List<string> Sunrise { get; set; } = new();

    [JsonPropertyName("sunset")]
    public List<string> Sunset { get; set; } = new();
}
