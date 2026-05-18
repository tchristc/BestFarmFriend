using BestFarmFriend.Core.Models;
using BestFarmFriend.Infrastructure.WeatherApi;

namespace BestFarmFriend.Tests.Unit.WeatherApi;

public class WeatherMapperTests
{
    private static OpenMeteoCurrentResponse MinimalResponse(double tempF = 72, int weatherCode = 0) => new()
    {
        Current = new OpenMeteoCurrent
        {
            Temperature2m = tempF,
            ApparentTemperature = tempF - 2,
            RelativeHumidity2m = 55,
            DewPoint2m = 50,
            Precipitation = 0,
            WeatherCode = weatherCode,
            WindSpeed10m = 5,
            WindGusts10m = 8,
            WindDirection10m = 180,
            SurfacePressure = 1013,
            Visibility = 16093,  // ~10 miles in meters
            UvIndex = 3
        }
    };

    [Fact]
    public void Map_BasicResponse_MapsTemperatureCorrectly()
    {
        var response = MinimalResponse(tempF: 75);

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.Equal(75, snapshot.TemperatureF);
    }

    [Fact]
    public void Map_BasicResponse_MapsHumidityCorrectly()
    {
        var response = MinimalResponse();

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.Equal(55, snapshot.HumidityPct);
    }

    [Fact]
    public void Map_BasicResponse_MapsWindCorrectly()
    {
        var response = MinimalResponse();

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.Equal(5, snapshot.WindSpeedMph);
        Assert.Equal(8, snapshot.WindGustMph);
    }

    [Fact]
    public void Map_SetsLocationTimezone()
    {
        var response = MinimalResponse();

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0, "America/Chicago");

        Assert.Equal("America/Chicago", snapshot.LocationTimeZone);
    }

    [Fact]
    public void Map_NoTimezone_DefaultsToEmpty()
    {
        var response = MinimalResponse();

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.Equal(string.Empty, snapshot.LocationTimeZone);
    }

    [Fact]
    public void Map_WithDailyData_MapsSunriseSunset()
    {
        var response = MinimalResponse();
        response.Daily = new OpenMeteoDaily
        {
            Time = ["2024-06-01"],
            Sunrise = ["2024-06-01T06:00"],
            Sunset = ["2024-06-01T20:30"],
            TemperatureMax = [85],
            TemperatureMin = [62],
            PrecipitationSum = [0],
            PrecipitationProbabilityMax = [10],
            WindSpeedMax = [12],
            WeatherCode = [0]
        };

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.NotEqual(default, snapshot.SunriseUtc);
        Assert.NotEqual(default, snapshot.SunsetUtc);
        Assert.True(snapshot.SunsetUtc > snapshot.SunriseUtc);
    }

    [Fact]
    public void Map_WithDailyData_MapsDailyHighLow()
    {
        var response = MinimalResponse();
        response.Daily = new OpenMeteoDaily
        {
            Time = ["2024-06-01"],
            Sunrise = ["2024-06-01T06:00"],
            Sunset = ["2024-06-01T20:00"],
            TemperatureMax = [88],
            TemperatureMin = [58],
            PrecipitationSum = [0],
            PrecipitationProbabilityMax = [0],
            WindSpeedMax = [8],
            WeatherCode = [0]
        };

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.Equal(88, snapshot.DailyHighF);
        Assert.Equal(58, snapshot.DailyLowF);
    }

    [Fact]
    public void Map_WithHourlyData_PopulatesHourlyForecast()
    {
        var response = MinimalResponse();
        response.Hourly = new OpenMeteoHourly
        {
            Time = ["2024-06-01T12:00", "2024-06-01T13:00"],
            Temperature2m = [74, 76],
            PrecipitationProbability = [5, 10],
            Precipitation = [0, 0.01],
            WindSpeed10m = [6, 7],
            WindGusts10m = [10, 12],
            WeatherCode = [1, 2]
        };

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.Equal(2, snapshot.HourlyForecast.Count);
        Assert.Equal(74, snapshot.HourlyForecast[0].TemperatureF);
    }

    [Theory]
    [InlineData(0, WeatherCondition.ClearSky)]
    [InlineData(1, WeatherCondition.MainlyClear)]
    [InlineData(2, WeatherCondition.PartlyCloudy)]
    [InlineData(3, WeatherCondition.Overcast)]
    [InlineData(45, WeatherCondition.Fog)]
    [InlineData(61, WeatherCondition.Rain)]
    [InlineData(65, WeatherCondition.HeavyRain)]
    [InlineData(71, WeatherCondition.Snow)]
    [InlineData(95, WeatherCondition.Thunderstorm)]
    public void MapWeatherCode_KnownCodes_MapsCorrectly(int code, WeatherCondition expected)
    {
        var condition = WeatherMapper.MapWeatherCode(code);

        Assert.Equal(expected, condition);
    }

    [Fact]
    public void MapWeatherCode_UnknownCode_ReturnsFallback()
    {
        var condition = WeatherMapper.MapWeatherCode(999);

        Assert.Equal(WeatherCondition.PartlyCloudy, condition);
    }

    [Fact]
    public void Map_VisibilityConverted_FromMetersToMiles()
    {
        var response = MinimalResponse();
        response.Current!.Visibility = 16093.4; // ~10 miles

        var snapshot = WeatherMapper.Map(response, 44.0, -92.0);

        Assert.True(snapshot.VisibilityMiles > 9.9 && snapshot.VisibilityMiles < 10.1);
    }

    [Fact]
    public void Map_NullResponse_DoesNotThrow()
    {
        var response = new OpenMeteoCurrentResponse(); // Current is null

        var ex = Record.Exception(() => WeatherMapper.Map(response, 44.0, -92.0));

        Assert.Null(ex);
    }
}
