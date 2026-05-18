using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Rules;

namespace BestFarmFriend.Tests.Unit.Rules;

public class PrecipitationRuleTests
{
    [Fact]
    public void Evaluate_NoPrecipNoForecast_ReturnsPass()
    {
        var result = PrecipitationRule.Evaluate(0, 0, 0, 0);

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.Equal(100, result.Score);
    }

    [Fact]
    public void Evaluate_RecentRain_ReturnsFail()
    {
        var result = PrecipitationRule.Evaluate(precipPast1hIn: 0.1, precipPast2hIn: 0.1, forecastNext1hIn: 0, forecastNext4hIn: 0);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void Evaluate_RainForecastWithin1Hour_ReturnsFail()
    {
        var result = PrecipitationRule.Evaluate(0, 0, forecastNext1hIn: 0.05, forecastNext4hIn: 0.05);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void Evaluate_RainForecastIn1To4Hours_ReturnsMarginal()
    {
        var result = PrecipitationRule.Evaluate(0, 0, forecastNext1hIn: 0, forecastNext4hIn: 0.1);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Fact]
    public void Evaluate_Factor_IsPrecipitation()
    {
        var result = PrecipitationRule.Evaluate(0, 0, 0, 0);

        Assert.Equal(SprayFactor.Precipitation, result.Factor);
    }

    [Fact]
    public void Evaluate_ZeroPast2h_ZeroNext1h_WithNext4hForecast_ReturnsMarginal()
    {
        var result = PrecipitationRule.Evaluate(precipPast1hIn: 0, precipPast2hIn: 0, forecastNext1hIn: 0, forecastNext4hIn: 0.25);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Evaluate_Past1hRainButNoPast2h_NoForecast_ReturnsPass()
    {
        // precipPast2hIn == 0 and forecastNext1hIn == 0 => clear
        var result = PrecipitationRule.Evaluate(precipPast1hIn: 0.05, precipPast2hIn: 0, forecastNext1hIn: 0, forecastNext4hIn: 0);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }
}
