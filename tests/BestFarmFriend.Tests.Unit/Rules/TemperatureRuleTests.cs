using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Rules;

namespace BestFarmFriend.Tests.Unit.Rules;

public class TemperatureRuleTests
{
    // -----------------------------------------------------------------------
    // Pass band: 45–90 °F inclusive
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(45)]    // exact lower Pass boundary
    [InlineData(45.1)]  // just inside Pass from below
    [InlineData(70)]    // mid-range
    [InlineData(89.9)]  // just inside Pass from above
    [InlineData(90)]    // exact upper Pass boundary (not > 90)
    public void Evaluate_IdealTemperature_ReturnsPassWithScore100(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.Equal(100, result.Score);
    }

    // -----------------------------------------------------------------------
    // Cool-Marginal band: 40–44.9… °F  (>= 40 and < 45)
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(40)]    // exact lower Marginal boundary (not < 40, so no Fail)
    [InlineData(40.1)]  // just above cold Fail threshold
    [InlineData(42)]    // mid cool-marginal
    [InlineData(44.9)]  // just below Pass threshold
    [InlineData(44.99)] // epsilon below Pass threshold
    public void Evaluate_CoolMarginal_ReturnsMarginalWithScore50(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    // -----------------------------------------------------------------------
    // Hot-Marginal band: 90.1–95 °F  (> 90 and <= 95)
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(90.1)]  // just above Pass upper boundary
    [InlineData(92)]    // mid hot-marginal
    [InlineData(94.9)]  // just below hot Fail threshold
    [InlineData(95)]    // exact upper Marginal boundary (not > 95, so no Fail)
    public void Evaluate_HotMarginal_ReturnsMarginalWithScore50(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.Equal(50, result.Score);
    }

    // -----------------------------------------------------------------------
    // Cold-Fail band: < 40 °F
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(39.9)]  // just below cold Marginal boundary
    [InlineData(20)]    // well below
    [InlineData(0)]     // freezing
    [InlineData(-10)]   // below freezing
    public void Evaluate_TooCold_ReturnsFailWithScore0(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    // -----------------------------------------------------------------------
    // Hot-Fail band: > 95 °F
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(95.1)]  // just above hot Marginal boundary
    [InlineData(100)]   // well above
    [InlineData(110)]   // extreme heat
    public void Evaluate_TooHot_ReturnsFailWithScore0(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.Equal(0, result.Score);
    }

    // -----------------------------------------------------------------------
    // Exact boundary values — explicit intent tests
    // -----------------------------------------------------------------------

    [Fact]
    public void Evaluate_Exactly39_9F_ReturnsColdFail()
    {
        var result = TemperatureRule.Evaluate(39.9);

        Assert.Equal(FactorStatus.Fail, result.Status);
    }

    [Fact]
    public void Evaluate_Exactly40F_ReturnsCoolMarginal()
    {
        // 40 < 40 is false → not Fail; 40 < 45 is true → Marginal
        var result = TemperatureRule.Evaluate(40);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Evaluate_Exactly45F_ReturnsPass()
    {
        // 45 < 45 is false → not Marginal; falls through to Pass
        var result = TemperatureRule.Evaluate(45);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Fact]
    public void Evaluate_Exactly90F_ReturnsPass()
    {
        // 90 > 90 is false → not Marginal; falls through to Pass
        var result = TemperatureRule.Evaluate(90);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Fact]
    public void Evaluate_Exactly95F_ReturnsHotMarginal()
    {
        // 95 > 95 is false → not Fail; 95 > 90 is true → Marginal
        var result = TemperatureRule.Evaluate(95);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Evaluate_Exactly95_1F_ReturnsHotFail()
    {
        // 95.1 > 95 is true → Fail
        var result = TemperatureRule.Evaluate(95.1);

        Assert.Equal(FactorStatus.Fail, result.Status);
    }

    // -----------------------------------------------------------------------
    // Result metadata
    // -----------------------------------------------------------------------

    [Fact]
    public void Evaluate_Factor_IsTemperature()
    {
        var result = TemperatureRule.Evaluate(70);

        Assert.Equal(SprayFactor.Temperature, result.Factor);
    }

    [Fact]
    public void Evaluate_CurrentValue_ContainsFormattedTemperature()
    {
        var result = TemperatureRule.Evaluate(72.5);

        Assert.Contains("72.5", result.CurrentValue);
    }

    [Theory]
    [InlineData(39.9)]  // cold fail
    [InlineData(42)]    // cool marginal
    [InlineData(70)]    // pass
    [InlineData(92)]    // hot marginal
    [InlineData(96)]    // hot fail
    public void Evaluate_AllBands_ReasonIsNotEmpty(double tempF)
    {
        var result = TemperatureRule.Evaluate(tempF);

        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
    }
}
