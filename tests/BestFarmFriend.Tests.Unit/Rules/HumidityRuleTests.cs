using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Rules;

namespace BestFarmFriend.Tests.Unit.Rules;

public class HumidityRuleTests
{
    [Theory]
    [InlineData(50, 70, 55)]   // normal conditions, gap = 15
    [InlineData(80, 75, 65)]   // high-ish but under 85, gap = 10
    [InlineData(85, 80, 70)]   // exactly 85 is boundary; 85 is NOT > 85, gap = 10
    public void Evaluate_AcceptableHumidity_ReturnsPass(double humidity, double tempF, double dewPoint)
    {
        var result = HumidityRule.Evaluate(humidity, tempF, dewPoint);

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.Equal(100, result.Score);
    }

    [Theory]
    [InlineData(86, 70, 60)]   // humidity > 85, gap = 10
    [InlineData(90, 75, 65)]   // humidity > 85, gap = 10
    public void Evaluate_HighHumidity_ReturnsMarginal(double humidity, double tempF, double dewPoint)
    {
        var result = HumidityRule.Evaluate(humidity, tempF, dewPoint);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Fact]
    public void Evaluate_DewPointGapAtOrBelow3_ReturnsMarginal()
    {
        // tempF = 65, dewPoint = 63 => gap = 2
        var result = HumidityRule.Evaluate(humidityPct: 70, tempF: 65, dewPointF: 63);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Fact]
    public void Evaluate_HumidityAbove95_ReturnsFail()
    {
        var result = HumidityRule.Evaluate(humidityPct: 96, tempF: 70, dewPointF: 65);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void Evaluate_HumidityExactly95_ReturnsMarginal()
    {
        // > 95 fails; == 95 does not fail — still marginal because 95 > 85
        var result = HumidityRule.Evaluate(humidityPct: 95, tempF: 70, dewPointF: 60);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Evaluate_Factor_IsHumidity()
    {
        var result = HumidityRule.Evaluate(50, 70, 55);

        Assert.Equal(SprayFactor.Humidity, result.Factor);
    }

    [Fact]
    public void Evaluate_Reason_MentionsDewPoint_WhenGapIsTight()
    {
        var result = HumidityRule.Evaluate(humidityPct: 70, tempF: 65, dewPointF: 63);

        Assert.Contains("ew", result.Reason); // "Dew point" or "dew"
    }
}
