using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Rules;

public static class PrecipitationRule
{
    public static SprayFactorResult Evaluate(double precipPast1hIn, double precipPast2hIn,
        double forecastNext1hIn, double forecastNext4hIn)
    {
        if (precipPast2hIn > 0 || forecastNext1hIn > 0)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Precipitation,
                CurrentValue = $"Past 1h: {precipPast1hIn:F2}\" | Next 1h forecast: {forecastNext1hIn:F2}\"",
                Status = FactorStatus.Fail,
                Score = 0,
                Reason = precipPast2hIn > 0
                    ? "Rain within past 2 hours — foliage likely wet."
                    : "Rain expected within 1 hour — spray won't have time to set."
            };

        if (forecastNext4hIn > 0)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Precipitation,
                CurrentValue = $"Past 1h: {precipPast1hIn:F2}\" | Next 4h forecast: {forecastNext4hIn:F2}\"",
                Status = FactorStatus.Marginal,
                Score = 50,
                Reason = $"Rain expected in the next 1–4 hours ({forecastNext4hIn:F2}\"). Use systemic products only."
            };

        return new SprayFactorResult
        {
            Factor = SprayFactor.Precipitation,
            CurrentValue = $"Past 1h: {precipPast1hIn:F2}\" | Next 4h: clear",
            Status = FactorStatus.Pass,
            Score = 100,
            Reason = "No rain in the past 2 hours or forecast next 4 hours."
        };
    }
}
