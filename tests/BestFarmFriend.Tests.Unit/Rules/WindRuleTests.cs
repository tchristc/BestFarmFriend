using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Rules;

namespace BestFarmFriend.Tests.Unit.Rules;

public class WindRuleTests
{
    [Fact]
    public void Evaluate_CalmWind_ReturnsPass()
    {
        var result = WindRule.Evaluate(windSpeedMph: 3, gustMph: 5);

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.Equal(100, result.Score);
    }

    [Theory]
    [InlineData(6, 10)]
    [InlineData(9, 14)]
    [InlineData(10, 15)]
    public void Evaluate_MarginalWind_ReturnsMarginal(double windSpeed, double gust)
    {
        var result = WindRule.Evaluate(windSpeed, gust);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Theory]
    [InlineData(11, 10)]  // wind over 10 triggers fail
    [InlineData(5, 16)]   // gust over 15 triggers fail
    [InlineData(15, 20)]  // both over limit
    public void Evaluate_HighWind_ReturnsFail(double windSpeed, double gust)
    {
        var result = WindRule.Evaluate(windSpeed, gust);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void Evaluate_GustExactly15_ReturnsPass()
    {
        // condition is gust > 15; exactly 15 does not fail, and wind = 4 <= 5 so Pass
        var result = WindRule.Evaluate(windSpeedMph: 4, gustMph: 15);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Fact]
    public void Evaluate_GustJustAboveLimit_ReturnsFail()
    {
        var result = WindRule.Evaluate(windSpeedMph: 4, gustMph: 15.1);

        Assert.Equal(FactorStatus.Fail, result.Status);
    }

    [Fact]
    public void Evaluate_WindExactlyAtLimit_ReturnsMarginal()
    {
        // wind = 10 is the boundary; > 10 fails
        var result = WindRule.Evaluate(windSpeedMph: 10, gustMph: 10);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Evaluate_Result_ContainsCurrentValue()
    {
        var result = WindRule.Evaluate(windSpeedMph: 3.5, gustMph: 7.2);

        Assert.Contains("3.5", result.CurrentValue);
        Assert.Contains("7.2", result.CurrentValue);
    }

    [Fact]
    public void Evaluate_Factor_IsWind()
    {
        var result = WindRule.Evaluate(5, 10);

        Assert.Equal(SprayFactor.Wind, result.Factor);
    }
}
