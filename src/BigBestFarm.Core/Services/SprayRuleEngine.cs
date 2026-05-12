using BigBestFarm.Core.Interfaces;
using BigBestFarm.Core.Models;
using BigBestFarm.Core.Rules;

namespace BigBestFarm.Core.Services;

public class SprayRuleEngine : ISprayRuleEngine
{
    public SprayReadinessResult Evaluate(WeatherSnapshot weather)
    {
        var now = DateTime.UtcNow;

        // Estimate precip in the past 2 hours from past1h (conservative)
        double precipPast2h = weather.PrecipPast1hIn;

        // Get forecast values for next 1h and next 4h from hourly forecast
        double forecastNext1h = 0;
        double forecastNext4h = 0;
        var upcoming = weather.HourlyForecast
            .Where(h => h.Time >= now && h.Time <= now.AddHours(4))
            .ToList();
        if (upcoming.Count > 0)
        {
            forecastNext1h = upcoming.FirstOrDefault()?.PrecipitationIn ?? 0;
            forecastNext4h = upcoming.Sum(h => h.PrecipitationIn);
        }

        var windResult = WindRule.Evaluate(weather.WindSpeedMph, weather.WindGustMph);
        var precipResult = PrecipitationRule.Evaluate(weather.PrecipPast1hIn, precipPast2h, forecastNext1h, forecastNext4h);
        var tempResult = TemperatureRule.Evaluate(weather.TemperatureF);
        var humidityResult = HumidityRule.Evaluate(weather.HumidityPct, weather.TemperatureF, weather.DewPointF);
        var timeResult = TimeOfDayRule.Evaluate(now, weather.SunriseUtc, weather.SunsetUtc);

        // Weighted score
        double score =
            windResult.Score * 0.30 +
            precipResult.Score * 0.25 +
            tempResult.Score * 0.20 +
            humidityResult.Score * 0.15 +
            timeResult.Score * 0.10;

        var band = score switch
        {
            >= 80 => SprayBand.Go,
            >= 60 => SprayBand.Caution,
            >= 40 => SprayBand.Marginal,
            _ => SprayBand.NoGo
        };

        var productResults = new List<ProductReadinessResult>
        {
            ProductRules.EvaluateCopper(weather),
            ProductRules.EvaluateSulfur(weather),
            ProductRules.EvaluateOil(weather),
            ProductRules.EvaluateInsecticide(weather),
            ProductRules.EvaluateHerbicide(weather)
        };

        return new SprayReadinessResult
        {
            Score = Math.Round(score, 1),
            Band = band,
            FactorResults = [windResult, precipResult, tempResult, humidityResult, timeResult],
            ProductResults = productResults,
            EvaluatedAt = now
        };
    }
}
