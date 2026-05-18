using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Rules;

namespace BestFarmFriend.Tests.Unit.Rules;

public class TemperatureRuleTests
{
    [Theory]
    [InlineData(45)]
    [InlineData(70)]
    [InlineData(90)]
    public void Evaluate_IdealTemperature_ReturnsPass(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.Equal(100, result.Score);
    }

    [Theory]
    [InlineData(40.1)]  // just above cold fail threshold
    [InlineData(44.9)]  // just below ideal start
    public void Evaluate_CoolMarginal_ReturnsMarginal(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Theory]
    [InlineData(90.1)]  // just above marginal hot start
    [InlineData(94.9)]  // just below hot fail threshold
    public void Evaluate_HotMarginal_ReturnsMarginal(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    [Theory]
    [InlineData(39.9)]  // just below cold fail threshold
    [InlineData(0)]
    [InlineData(-10)]
    public void Evaluate_TooColde_ReturnsFail(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    [Theory]
    [InlineData(95.1)]
    [InlineData(110)]
    public void Evaluate_TooHot_ReturnsFail(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    [Fact]
    public void Evaluate_ExactlyAt40F_ReturnsFail()
    {
        // < 40 fails; == 40 is marginal (40 < 40 is false)
        var result = TemperatureRule.Evaluate(40);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Evaluate_ExactlyAt95F_ReturnsFail()
    {
        // > 95 fails; == 95 does not fail
        var result = TemperatureRule.Evaluate(95);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Evaluate_Factor_IsTemperature()
    {
        var result = TemperatureRule.Evaluate(70);

        Assert.Equal(SprayFactor.Temperature, result.Factor);
    }

    [Fact]
    public void Evaluate_Result_ContainsTemperatureValue()
    {
        var result = TemperatureRule.Evaluate(72.5);

        Assert.Contains("72.5", result.CurrentValue);
    }
}
