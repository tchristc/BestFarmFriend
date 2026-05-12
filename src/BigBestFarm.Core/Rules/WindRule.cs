using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Rules;

public static class WindRule
{
    public static SprayFactorResult Evaluate(double windSpeedMph, double gustMph)
    {
        if (gustMph > 15 || windSpeedMph > 10)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Wind,
                CurrentValue = $"{windSpeedMph:F1} mph (gust {gustMph:F1} mph)",
                Status = FactorStatus.Fail,
                Score = 0,
                Reason = gustMph > 15
                    ? $"Gusts of {gustMph:F1} mph exceed the 15 mph limit."
                    : $"Wind speed {windSpeedMph:F1} mph exceeds 10 mph limit."
            };

        if (windSpeedMph > 5)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Wind,
                CurrentValue = $"{windSpeedMph:F1} mph (gust {gustMph:F1} mph)",
                Status = FactorStatus.Marginal,
                Score = 50,
                Reason = $"Wind {windSpeedMph:F1} mph is marginal (6–10 mph). Short spray windows only."
            };

        return new SprayFactorResult
        {
            Factor = SprayFactor.Wind,
            CurrentValue = $"{windSpeedMph:F1} mph (gust {gustMph:F1} mph)",
            Status = FactorStatus.Pass,
            Score = 100,
            Reason = "Wind speed is ideal for spraying."
        };
    }
}
