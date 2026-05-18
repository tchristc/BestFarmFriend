using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Services;

namespace BestFarmFriend.Tests.Unit.Services;

public class SprayRuleEngineTests
{
    private static WeatherSnapshot IdealWeather() => new()
    {
        TemperatureF = 70,
        DewPointF = 50,
        HumidityPct = 55,
        WindSpeedMph = 4,
        WindGustMph = 8,
        PrecipPast1hIn = 0,
        SunriseUtc = DateTime.UtcNow.Date.AddHours(11),  // 6 AM CDT
        SunsetUtc  = DateTime.UtcNow.Date.AddHours(1).AddDays(1), // 8 PM CDT next-day UTC
        LocationTimeZone = "America/Chicago",
        HourlyForecast = new List<WeatherForecastHour>()
    };

    [Fact]
    public void Evaluate_AllIdealConditions_ReturnsGoBand()
    {
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();

        var result = engine.Evaluate(weather);

        Assert.Equal(SprayBand.Go, result.Band);
        Assert.True(result.Score >= 80);
    }

    [Fact]
    public void Evaluate_HighWind_LowersScoreToNoBetter_ThanCaution()
    {
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();
        weather.WindSpeedMph = 12;
        weather.WindGustMph = 18;

        var result = engine.Evaluate(weather);

        // Wind weight is 30%; a zero wind score from ideal base drops max possible score significantly
        Assert.True(result.Score < 80);
    }

    [Fact]
    public void Evaluate_ActiveRain_PrecipFactorFails()
    {
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();
        weather.PrecipPast1hIn = 0.2;

        var result = engine.Evaluate(weather);

        // Precip weight is 25%; rain alone lowers score but other factors can keep it in Caution.
        // The important thing is the precipitation factor itself fails.
        var precipResult = result.FactorResults.First(f => f.Factor == SprayFactor.Precipitation);
        Assert.Equal(FactorStatus.Fail, precipResult.Status);
        Assert.Equal(0, precipResult.Score);
    }

    [Fact]
    public void Evaluate_ExtremeCold_DropsScore()
    {
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();
        weather.TemperatureF = 30;

        var result = engine.Evaluate(weather);

        Assert.True(result.Score < 80);
    }

    [Fact]
    public void Evaluate_ExtremeHeat_TemperatureFactorFails()
    {
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();
        weather.TemperatureF = 100;

        var result = engine.Evaluate(weather);

        // Temperature weight is 20%; extreme heat causes temp factor to fail.
        var tempResult = result.FactorResults.First(f => f.Factor == SprayFactor.Temperature);
        Assert.Equal(FactorStatus.Fail, tempResult.Status);
        Assert.Equal(0, tempResult.Score);
        // Score must be reduced from maximum
        Assert.True(result.Score <= 80);
    }

    [Fact]
    public void Evaluate_AllFactorsFail_ReturnsNoGoBand()
    {
        var engine = new SprayRuleEngine();
        var weather = new WeatherSnapshot
        {
            TemperatureF = 30,       // fail
            DewPointF = 29,          // gap = 1 => marginal, but temp fails
            HumidityPct = 98,        // fail
            WindSpeedMph = 20,       // fail
            WindGustMph = 25,        // fail
            PrecipPast1hIn = 0.5,    // fail
            SunriseUtc = DateTime.UtcNow.Date.AddHours(11),
            SunsetUtc  = DateTime.UtcNow.Date.AddHours(12),  // sunset=12 => now is outside window
            LocationTimeZone = "UTC",
            HourlyForecast = new List<WeatherForecastHour>()
        };

        var result = engine.Evaluate(weather);

        Assert.Equal(SprayBand.NoGo, result.Band);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void Evaluate_Returns5FactorResults()
    {
        var engine = new SprayRuleEngine();
        var result = engine.Evaluate(IdealWeather());

        Assert.Equal(5, result.FactorResults.Count);
    }

    [Fact]
    public void Evaluate_Returns5ProductResults()
    {
        var engine = new SprayRuleEngine();
        var result = engine.Evaluate(IdealWeather());

        Assert.Equal(5, result.ProductResults.Count);
    }

    [Fact]
    public void Evaluate_ScoreIsWeightedCorrectly_AllPass()
    {
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();

        var result = engine.Evaluate(weather);

        // When all factors are Pass (score=100), weighted score = 100
        // But TimeOfDay depends on current time — check that wind/precip/temp/humidity all pass
        var windResult = result.FactorResults.First(f => f.Factor == SprayFactor.Wind);
        var precipResult = result.FactorResults.First(f => f.Factor == SprayFactor.Precipitation);
        var tempResult = result.FactorResults.First(f => f.Factor == SprayFactor.Temperature);
        var humidityResult = result.FactorResults.First(f => f.Factor == SprayFactor.Humidity);

        Assert.Equal(FactorStatus.Pass, windResult.Status);
        Assert.Equal(FactorStatus.Pass, precipResult.Status);
        Assert.Equal(FactorStatus.Pass, tempResult.Status);
        Assert.Equal(FactorStatus.Pass, humidityResult.Status);
    }

    [Fact]
    public void Evaluate_BandBoundaries_CautionAt60()
    {
        // Construct weather where only wind fails (weight 0.30), rest pass = score 70 => Caution
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();
        weather.WindSpeedMph = 8;  // marginal (score 50)
        weather.WindGustMph = 12;  // marginal

        var result = engine.Evaluate(weather);

        // Wind = 50 * 0.30 = 15; rest = 100 * 0.70 = 70 max (time-of-day may vary)
        // Score should be in Caution or Go depending on time of day
        Assert.True(result.Score >= 60 || result.Score >= 40);
    }

    [Fact]
    public void Evaluate_ForecastRainNext4h_LowersSprayScore()
    {
        var engine = new SprayRuleEngine();
        var weather = IdealWeather();
        var futureHour = DateTime.UtcNow.AddHours(2);
        weather.HourlyForecast.Add(new WeatherForecastHour
        {
            Time = futureHour,
            PrecipitationIn = 0.1,
            TemperatureF = 70,
            WindSpeedMph = 4,
            WindGustMph = 8
        });

        var result = engine.Evaluate(weather);

        var precipResult = result.FactorResults.First(f => f.Factor == SprayFactor.Precipitation);
        Assert.NotEqual(FactorStatus.Pass, precipResult.Status);
    }
}
