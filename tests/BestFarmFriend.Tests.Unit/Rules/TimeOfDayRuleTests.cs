using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Rules;

namespace BestFarmFriend.Tests.Unit.Rules;

public class TimeOfDayRuleTests
{
    // CDT = UTC-5
    // Sunrise: 6 AM CDT = 11:00 UTC on June 1
    // Sunset:  8 PM CDT = 01:00 UTC on June 2
    private static readonly DateTime SunriseUtc = new DateTime(2024, 6, 1, 11, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime SunsetUtc  = new DateTime(2024, 6, 2,  1, 0, 0, DateTimeKind.Utc);
    private const string Tz = "America/Chicago";

    // Ideal window: 1h after local sunrise (7 AM CDT) to 2h before local sunset (6 PM CDT)
    // UtcFor maps a CDT hour on June 1 to its UTC equivalent.
    private static DateTime UtcFor(int hourCdt, int minuteCdt = 0) =>
        new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc).AddHours(hourCdt + 5).AddMinutes(minuteCdt);

    [Theory]
    [InlineData(9)]   // 9 AM CDT — ideal window
    [InlineData(12)]  // noon
    [InlineData(17)]  // 5 PM CDT — still in ideal window (sunset 8 PM, -2 = 6 PM)
    public void Evaluate_IdealWindow_ReturnsPass(int hourCdt)
    {
        var now = UtcFor(hourCdt);
        var result = TimeOfDayRule.Evaluate(now, SunriseUtc, SunsetUtc, Tz);

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.Equal(100, result.Score);
    }

    [Theory]
    [InlineData(6, 0)]   // exactly at sunrise — within 1h of sunrise
    [InlineData(6, 30)]  // 6:30 AM CDT — still marginal
    public void Evaluate_WithinOneHourOfSunrise_ReturnsMarginal(int hourCdt, int minuteCdt)
    {
        var now = UtcFor(hourCdt, minuteCdt);
        var result = TimeOfDayRule.Evaluate(now, SunriseUtc, SunsetUtc, Tz);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Theory]
    [InlineData(18, 1)]   // 6:01 PM CDT — just past 2h-before-sunset boundary
    [InlineData(19, 0)]   // 7 PM CDT
    public void Evaluate_Within2HoursOfSunset_ReturnsMarginal(int hourCdt, int minuteCdt)
    {
        var now = UtcFor(hourCdt, minuteCdt);
        var result = TimeOfDayRule.Evaluate(now, SunriseUtc, SunsetUtc, Tz);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Theory]
    [InlineData(3)]   // 3 AM CDT — nighttime
    [InlineData(21)]  // 9 PM CDT — after sunset
    public void Evaluate_Nighttime_ReturnsFail(int hourCdt)
    {
        var now = UtcFor(hourCdt);
        var result = TimeOfDayRule.Evaluate(now, SunriseUtc, SunsetUtc, Tz);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void Evaluate_Factor_IsTimeOfDay()
    {
        var now = UtcFor(10);
        var result = TimeOfDayRule.Evaluate(now, SunriseUtc, SunsetUtc, Tz);

        Assert.Equal(SprayFactor.TimeOfDay, result.Factor);
    }

    [Fact]
    public void Evaluate_NoTimezone_FallsBackWithoutThrowing()
    {
        var now = DateTime.UtcNow;
        var sunrise = now.Date.AddHours(11);
        var sunset = now.Date.AddHours(23);

        var ex = Record.Exception(() => TimeOfDayRule.Evaluate(now, sunrise, sunset, ""));
        Assert.Null(ex);
    }
}
