using BestFarmFriend.Core.Models;
using TimeZoneConverter;

namespace BestFarmFriend.Core.Rules;

public static class TimeOfDayRule
{
    public static SprayFactorResult Evaluate(DateTime now, DateTime sunriseUtc, DateTime sunsetUtc, string locationTimeZone = "")
    {
        var tzi = GetTimeZoneInfo(locationTimeZone);
        var localNow     = TimeZoneInfo.ConvertTimeFromUtc(now.Kind == DateTimeKind.Utc ? now : now.ToUniversalTime(), tzi);
        var localSunrise = TimeZoneInfo.ConvertTimeFromUtc(sunriseUtc.Kind == DateTimeKind.Utc ? sunriseUtc : sunriseUtc.ToUniversalTime(), tzi);
        var localSunset  = TimeZoneInfo.ConvertTimeFromUtc(sunsetUtc.Kind == DateTimeKind.Utc ? sunsetUtc : sunsetUtc.ToUniversalTime(), tzi);

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

    private static TimeZoneInfo GetTimeZoneInfo(string? tz)
    {
        if (!string.IsNullOrWhiteSpace(tz))
        {
            try { return TZConvert.GetTimeZoneInfo(tz); } catch { }
        }
        return TimeZoneInfo.Local;
    }
}
