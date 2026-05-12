using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Infrastructure.WeatherApi;

public static class WeatherMapper
{
    public static WeatherSnapshot Map(OpenMeteoCurrentResponse response, double latitude, double longitude, string locationTimeZone = "")
    {
        var current = response.Current ?? new OpenMeteoCurrent();
        var daily = response.Daily;

        DateTime sunriseUtc = DateTime.UtcNow.Date.AddHours(6);
        DateTime sunsetUtc = DateTime.UtcNow.Date.AddHours(20);
        double dailyHigh = current.Temperature2m;
        double dailyLow = current.Temperature2m;

        if (daily != null && daily.Time.Count > 0)
        {
            if (daily.Sunrise.Count > 0 && DateTime.TryParse(daily.Sunrise[0], out var sr))
                sunriseUtc = sr.ToUniversalTime();
            if (daily.Sunset.Count > 0 && DateTime.TryParse(daily.Sunset[0], out var ss))
                sunsetUtc = ss.ToUniversalTime();
            if (daily.TemperatureMax.Count > 0) dailyHigh = daily.TemperatureMax[0];
            if (daily.TemperatureMin.Count > 0) dailyLow = daily.TemperatureMin[0];
        }

        var snapshot = new WeatherSnapshot
        {
            ObservedAt = DateTime.UtcNow,
            TemperatureF = current.Temperature2m,
            FeelsLikeF = current.ApparentTemperature,
            DewPointF = current.DewPoint2m,
            DailyHighF = dailyHigh,
            DailyLowF = dailyLow,
            HumidityPct = current.RelativeHumidity2m,
            WindSpeedMph = current.WindSpeed10m,
            WindGustMph = current.WindGusts10m,
            WindDirectionDeg = current.WindDirection10m,
            PrecipitationRateInPerHr = current.Precipitation,
            PrecipPast1hIn = current.Precipitation,
            PressureHpa = current.SurfacePressure,
            UvIndex = current.UvIndex,
            VisibilityMiles = current.Visibility / 1609.34,
            Condition = MapWeatherCode(current.WeatherCode),
            SunriseUtc = sunriseUtc,
            SunsetUtc = sunsetUtc,
            LocationTimeZone = locationTimeZone
        };

        if (response.Hourly != null)
        {
            var hourly = response.Hourly;
            for (int i = 0; i < hourly.Time.Count; i++)
            {
                if (!DateTime.TryParse(hourly.Time[i], out var t)) continue;
                snapshot.HourlyForecast.Add(new WeatherForecastHour
                {
                    Time = t.ToUniversalTime(),
                    TemperatureF = i < hourly.Temperature2m.Count ? hourly.Temperature2m[i] : 0,
                    PrecipitationProbabilityPct = i < hourly.PrecipitationProbability.Count ? hourly.PrecipitationProbability[i] : 0,
                    PrecipitationIn = i < hourly.Precipitation.Count ? hourly.Precipitation[i] : 0,
                    WindSpeedMph = i < hourly.WindSpeed10m.Count ? hourly.WindSpeed10m[i] : 0,
                    WindGustMph = i < hourly.WindGusts10m.Count ? hourly.WindGusts10m[i] : 0,
                    Condition = i < hourly.WeatherCode.Count ? MapWeatherCode(hourly.WeatherCode[i]) : WeatherCondition.ClearSky
                });
            }

            // Compute past 24h precip from hourly
            var past24h = snapshot.HourlyForecast
                .Where(h => h.Time >= DateTime.UtcNow.AddHours(-24) && h.Time <= DateTime.UtcNow)
                .Sum(h => h.PrecipitationIn);
            snapshot.PrecipPast24hIn = past24h;
        }

        if (daily != null)
        {
            for (int i = 0; i < daily.Time.Count; i++)
            {
                if (!DateOnly.TryParse(daily.Time[i], out var d)) continue;
                DateTime sr2 = DateTime.UtcNow;
                DateTime ss2 = DateTime.UtcNow;
                if (i < daily.Sunrise.Count) DateTime.TryParse(daily.Sunrise[i], out sr2);
                if (i < daily.Sunset.Count) DateTime.TryParse(daily.Sunset[i], out ss2);

                snapshot.DailyForecast.Add(new WeatherForecastDay
                {
                    Date = d,
                    HighF = i < daily.TemperatureMax.Count ? daily.TemperatureMax[i] : 0,
                    LowF = i < daily.TemperatureMin.Count ? daily.TemperatureMin[i] : 0,
                    PrecipitationSumIn = i < daily.PrecipitationSum.Count ? daily.PrecipitationSum[i] : 0,
                    PrecipProbabilityPct = i < daily.PrecipitationProbabilityMax.Count ? daily.PrecipitationProbabilityMax[i] : 0,
                    WindSpeedMaxMph = i < daily.WindSpeedMax.Count ? daily.WindSpeedMax[i] : 0,
                    Condition = i < daily.WeatherCode.Count ? MapWeatherCode(daily.WeatherCode[i]) : WeatherCondition.ClearSky,
                    SunriseUtc = sr2.ToUniversalTime(),
                    SunsetUtc = ss2.ToUniversalTime()
                });
            }
        }

        return snapshot;
    }

    public static WeatherCondition MapWeatherCode(int code) => code switch
    {
        0 => WeatherCondition.ClearSky,
        1 => WeatherCondition.MainlyClear,
        2 => WeatherCondition.PartlyCloudy,
        3 => WeatherCondition.Overcast,
        45 or 48 => WeatherCondition.Fog,
        51 or 53 or 55 => WeatherCondition.Drizzle,
        61 or 63 => WeatherCondition.Rain,
        65 or 67 => WeatherCondition.HeavyRain,
        71 or 73 or 75 or 77 => WeatherCondition.Snow,
        80 or 81 or 82 => WeatherCondition.Rain,
        85 or 86 => WeatherCondition.Snow,
        95 or 96 or 99 => WeatherCondition.Thunderstorm,
        56 or 57 => WeatherCondition.Sleet,
        _ => WeatherCondition.PartlyCloudy
    };
}
