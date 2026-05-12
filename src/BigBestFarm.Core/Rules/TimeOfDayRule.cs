using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Rules;

public static class TimeOfDayRule
{
    public static SprayFactorResult Evaluate(DateTime now, DateTime sunriseUtc, DateTime sunsetUtc)
    {
        var localNow = now.ToLocalTime();
        var localSunrise = sunriseUtc.ToLocalTime();
        var localSunset = sunsetUtc.ToLocalTime();

        var idealStart = localSunrise.AddHours(1);
        var idealEnd = localSunset.AddHours(-2);
        var marginalStart = localSunrise;
        var marginalEnd = localSunset;

        if (localNow < marginalStart || localNow > marginalEnd)
            return new SprayFactorResult
            {
                Factor = SprayFactor.TimeOfDay,
                CurrentValue = $"{localNow:h:mm tt}",
                Status = FactorStatus.Fail,
                Score = 0,
                Reason = "Nighttime — poor visibility, dew risk."
            };

        if (localNow < idealStart)
            return new SprayFactorResult
            {
                Factor = SprayFactor.TimeOfDay,
                CurrentValue = $"{localNow:h:mm tt}",
                Status = FactorStatus.Marginal,
                Score = 50,
                Reason = "Within 1 hour of sunrise — morning dew may still be present."
            };

        if (localNow > idealEnd)
            return new SprayFactorResult
            {
                Factor = SprayFactor.TimeOfDay,
                CurrentValue = $"{localNow:h:mm tt}",
                Status = FactorStatus.Marginal,
                Score = 50,
                Reason = "Within 2 hours of sunset — evening dew may form before product dries."
            };

        return new SprayFactorResult
        {
            Factor = SprayFactor.TimeOfDay,
            CurrentValue = $"{localNow:h:mm tt}",
            Status = FactorStatus.Pass,
            Score = 100,
            Reason = "Ideal spray window (1 hr after sunrise to 2 hrs before sunset)."
        };
    }
}
