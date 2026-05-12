using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Core.Rules;

public static class TemperatureRule
{
    public static SprayFactorResult Evaluate(double tempF)
    {
        if (tempF > 95)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Temperature,
                CurrentValue = $"{tempF:F1}°F",
                Status = FactorStatus.Fail,
                Score = 0,
                Reason = $"Temperature {tempF:F1}°F exceeds 95°F — phytotoxicity risk."
            };

        if (tempF < 40)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Temperature,
                CurrentValue = $"{tempF:F1}°F",
                Status = FactorStatus.Fail,
                Score = 0,
                Reason = $"Temperature {tempF:F1}°F is below 40°F — do not apply fungicides or copper."
            };

        if (tempF > 90 || tempF < 45)
            return new SprayFactorResult
            {
                Factor = SprayFactor.Temperature,
                CurrentValue = $"{tempF:F1}°F",
                Status = FactorStatus.Marginal,
                Score = 50,
                Reason = tempF > 90
                    ? $"Temperature {tempF:F1}°F is high — spray early morning or evening only."
                    : $"Temperature {tempF:F1}°F is cool — some products may have reduced efficacy."
            };

        return new SprayFactorResult
        {
            Factor = SprayFactor.Temperature,
            CurrentValue = $"{tempF:F1}°F",
            Status = FactorStatus.Pass,
            Score = 100,
            Reason = "Temperature is within ideal spray range (45–90°F)."
        };
    }
}
