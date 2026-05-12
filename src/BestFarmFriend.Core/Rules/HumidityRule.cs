using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Core.Rules;

public static class HumidityRule
{
    public static SprayFactorResult Evaluate(double humidityPct, double tempF, double dewPointF)
    {
        double dewPointGap = tempF - dewPointF;

        if (humidityPct > 95)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Humidity,
                CurrentValue = $"{humidityPct:F0}% RH",
                Status = FactorStatus.Fail,
                Score = 0,
                Reason = $"Humidity {humidityPct:F0}% is above 95% — foliage is wet or near-wet."
            };

        if (humidityPct > 85 || dewPointGap <= 3)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Humidity,
                CurrentValue = $"{humidityPct:F0}% RH | Dew gap: {dewPointGap:F1}°F",
                Status = FactorStatus.Marginal,
                Score = 50,
                Reason = dewPointGap <= 3
                    ? $"Dew point within {dewPointGap:F1}°F of air temp — dew likely forming."
                    : $"Humidity {humidityPct:F0}% is high (85–95%) — disease pressure elevated."
            };

        return new SprayFactorResult
        {
            Factor = SprayFactor.Humidity,
            CurrentValue = $"{humidityPct:F0}% RH | Dew gap: {dewPointGap:F1}°F",
            Status = FactorStatus.Pass,
            Score = 100,
            Reason = "Humidity is within acceptable range for spraying."
        };
    }
}
