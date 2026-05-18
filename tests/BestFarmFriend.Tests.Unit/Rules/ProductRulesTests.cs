using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Rules;

namespace BestFarmFriend.Tests.Unit.Rules;

public class ProductRulesTests
{
    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    private static WeatherSnapshot BaseWeather() => new()
    {
        TemperatureF = 65,
        HumidityPct = 60,
        WindSpeedMph = 3,
        WindGustMph = 5,
        PrecipPast1hIn = 0,
        PrecipPast24hIn = 0,
        PrecipitationRateInPerHr = 0
    };

    // -----------------------------------------------------------------------
    // EvaluateCopper
    // Boundaries: tempF < 40 → Fail | 40–44.9 → Marginal | 45+ → Pass
    //             humidity > 90 → Fail | precipPast1h > 0 → Fail
    // -----------------------------------------------------------------------

    [Fact]
    public void Copper_IdealConditions_ReturnsPass()
    {
        var result = ProductRules.EvaluateCopper(BaseWeather());

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.True(result.IsRecommended);
        Assert.Equal(ProductType.Copper, result.ProductType);
    }

    [Theory]
    [InlineData(39.9)]  // just below cold threshold
    [InlineData(20)]
    public void Copper_TempBelow40_ReturnsFail(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateCopper(w);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.False(result.IsRecommended);
    }

    [Fact]
    public void Copper_TempExactly40_ReturnsMarginal()
    {
        // 40 < 40 is false (no Fail); 40 >= 40 && < 45 → Marginal
        var w = BaseWeather(); w.TemperatureF = 40;

        var result = ProductRules.EvaluateCopper(w);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Theory]
    [InlineData(40)]    // exact lower Marginal boundary
    [InlineData(40.1)]  // just inside Marginal
    [InlineData(44.9)]  // just below Pass boundary
    public void Copper_TempMarginalRange_ReturnsMarginal(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateCopper(w);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Fact]
    public void Copper_TempExactly45_ReturnsPass()
    {
        // 45 >= 45 → exits marginal check → Pass
        var w = BaseWeather(); w.TemperatureF = 45;

        var result = ProductRules.EvaluateCopper(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Fact]
    public void Copper_HumidityAbove90_ReturnsFail()
    {
        var w = BaseWeather(); w.HumidityPct = 90.1;

        var result = ProductRules.EvaluateCopper(w);

        Assert.Equal(FactorStatus.Fail, result.Status);
    }

    [Fact]
    public void Copper_HumidityExactly90_ReturnsPass()
    {
        // > 90 is false at 90 → not Fail
        var w = BaseWeather(); w.HumidityPct = 90;

        var result = ProductRules.EvaluateCopper(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Theory]
    [InlineData(0.01)]  // any rain in past 1h
    [InlineData(0.5)]
    public void Copper_RecentRain_ReturnsFail(double precipPast1h)
    {
        var w = BaseWeather(); w.PrecipPast1hIn = precipPast1h;

        var result = ProductRules.EvaluateCopper(w);

        Assert.Equal(FactorStatus.Fail, result.Status);
    }

    [Fact]
    public void Copper_ZeroPrecip_DoesNotFail()
    {
        var w = BaseWeather(); w.PrecipPast1hIn = 0;

        var result = ProductRules.EvaluateCopper(w);

        Assert.NotEqual(FactorStatus.Fail, result.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateSulfur
    // Boundaries: tempF > 90 → Fail | tempF < 50 → Marginal | else → Pass
    // -----------------------------------------------------------------------

    [Fact]
    public void Sulfur_IdealConditions_ReturnsPass()
    {
        var result = ProductRules.EvaluateSulfur(BaseWeather());

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.True(result.IsRecommended);
        Assert.Equal(ProductType.Sulfur, result.ProductType);
    }

    [Theory]
    [InlineData(90.1)]  // just above hot threshold
    [InlineData(100)]
    public void Sulfur_TempAbove90_ReturnsFail(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateSulfur(w);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.False(result.IsRecommended);
    }

    [Fact]
    public void Sulfur_TempExactly90_ReturnsPass()
    {
        // 90 > 90 is false → not Fail; 90 < 50 is false → not Marginal → Pass
        var w = BaseWeather(); w.TemperatureF = 90;

        var result = ProductRules.EvaluateSulfur(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Theory]
    [InlineData(49.9)]  // just below warm threshold
    [InlineData(32)]
    [InlineData(0)]
    public void Sulfur_TempBelow50_ReturnsMarginal(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateSulfur(w);

        Assert.Equal(FactorStatus.Marginal, result.Status);
        Assert.False(result.IsRecommended);
    }

    [Fact]
    public void Sulfur_TempExactly50_ReturnsPass()
    {
        // 50 < 50 is false → not Marginal → Pass
        var w = BaseWeather(); w.TemperatureF = 50;

        var result = ProductRules.EvaluateSulfur(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateOil
    // Boundaries: tempF < 32 || > 85 → Fail | 32–39.9 → Marginal | 40–85 → Pass
    // -----------------------------------------------------------------------

    [Fact]
    public void Oil_IdealConditions_ReturnsPass()
    {
        var result = ProductRules.EvaluateOil(BaseWeather());

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.True(result.IsRecommended);
        Assert.Equal(ProductType.Oil, result.ProductType);
    }

    [Theory]
    [InlineData(31.9)]  // just below freezing
    [InlineData(0)]
    [InlineData(85.1)]  // just above hot threshold
    [InlineData(100)]
    public void Oil_OutsideRange_ReturnsFail(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateOil(w);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.False(result.IsRecommended);
    }

    [Fact]
    public void Oil_TempExactly32_ReturnsMarginal()
    {
        // 32 < 32 is false → not Fail; 32 >= 32 && < 40 → Marginal
        var w = BaseWeather(); w.TemperatureF = 32;

        var result = ProductRules.EvaluateOil(w);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Theory]
    [InlineData(32)]    // exact lower Marginal boundary
    [InlineData(35)]    // mid low range
    [InlineData(39.9)]  // just below Pass boundary
    public void Oil_TempLowMarginalRange_ReturnsMarginal(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateOil(w);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Theory]
    [InlineData(40)]    // exact lower Pass boundary
    [InlineData(65)]    // mid range
    [InlineData(85)]    // exact upper Pass boundary
    public void Oil_TempPassRange_ReturnsPass(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateOil(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Fact]
    public void Oil_TempExactly85_ReturnsPass()
    {
        // 85 > 85 is false → not Fail → Pass
        var w = BaseWeather(); w.TemperatureF = 85;

        var result = ProductRules.EvaluateOil(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateInsecticide
    // Boundaries: wind > 10 || gust > 15 → Fail | temp < 45 || > 95 → Marginal | else → Pass
    // -----------------------------------------------------------------------

    [Fact]
    public void Insecticide_IdealConditions_ReturnsPass()
    {
        var result = ProductRules.EvaluateInsecticide(BaseWeather());

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.True(result.IsRecommended);
        Assert.Equal(ProductType.Insecticide, result.ProductType);
    }

    [Theory]
    [InlineData(10.1, 5)]   // wind just above threshold
    [InlineData(15, 5)]     // wind well above
    [InlineData(3, 15.1)]   // gust just above threshold
    [InlineData(3, 20)]     // gust well above
    public void Insecticide_HighWindOrGust_ReturnsFail(double windMph, double gustMph)
    {
        var w = BaseWeather(); w.WindSpeedMph = windMph; w.WindGustMph = gustMph;

        var result = ProductRules.EvaluateInsecticide(w);

        Assert.Equal(FactorStatus.Fail, result.Status);
    }

    [Fact]
    public void Insecticide_WindExactly10_DoesNotFail()
    {
        // 10 > 10 is false → not Fail
        var w = BaseWeather(); w.WindSpeedMph = 10; w.WindGustMph = 5;

        var result = ProductRules.EvaluateInsecticide(w);

        Assert.NotEqual(FactorStatus.Fail, result.Status);
    }

    [Fact]
    public void Insecticide_GustExactly15_DoesNotFail()
    {
        // 15 > 15 is false → not Fail
        var w = BaseWeather(); w.WindSpeedMph = 3; w.WindGustMph = 15;

        var result = ProductRules.EvaluateInsecticide(w);

        Assert.NotEqual(FactorStatus.Fail, result.Status);
    }

    [Theory]
    [InlineData(44.9)]  // just below lower temp threshold
    [InlineData(20)]
    [InlineData(95.1)]  // just above upper temp threshold
    [InlineData(110)]
    public void Insecticide_TempOutsideIdeal_ReturnsMarginal(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateInsecticide(w);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Theory]
    [InlineData(45)]    // exact lower Pass boundary
    [InlineData(70)]    // mid range
    [InlineData(95)]    // exact upper Pass boundary
    public void Insecticide_TempIdealRange_ReturnsPass(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateInsecticide(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    // -----------------------------------------------------------------------
    // EvaluateHerbicide
    // Boundaries: wind > 5 → Fail | temp < 50 || > 85 → Marginal | else → Pass
    // -----------------------------------------------------------------------

    [Fact]
    public void Herbicide_IdealConditions_ReturnsPass()
    {
        var result = ProductRules.EvaluateHerbicide(BaseWeather());

        Assert.Equal(FactorStatus.Pass, result.Status);
        Assert.True(result.IsRecommended);
        Assert.Equal(ProductType.Herbicide, result.ProductType);
    }

    [Theory]
    [InlineData(5.1)]   // just above strict limit
    [InlineData(10)]
    [InlineData(20)]
    public void Herbicide_WindAbove5_ReturnsFail(double windMph)
    {
        var w = BaseWeather(); w.WindSpeedMph = windMph;

        var result = ProductRules.EvaluateHerbicide(w);

        Assert.Equal(FactorStatus.Fail, result.Status);
        Assert.False(result.IsRecommended);
    }

    [Fact]
    public void Herbicide_WindExactly5_DoesNotFail()
    {
        // 5 > 5 is false → not Fail
        var w = BaseWeather(); w.WindSpeedMph = 5;

        var result = ProductRules.EvaluateHerbicide(w);

        Assert.NotEqual(FactorStatus.Fail, result.Status);
    }

    [Theory]
    [InlineData(49.9)]  // just below lower temp threshold
    [InlineData(20)]
    [InlineData(85.1)]  // just above upper temp threshold
    [InlineData(100)]
    public void Herbicide_TempOutsideIdeal_ReturnsMarginal(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateHerbicide(w);

        Assert.Equal(FactorStatus.Marginal, result.Status);
    }

    [Theory]
    [InlineData(50)]    // exact lower Pass boundary
    [InlineData(65)]    // mid range
    [InlineData(85)]    // exact upper Pass boundary
    public void Herbicide_TempIdealRange_ReturnsPass(double tempF)
    {
        var w = BaseWeather(); w.TemperatureF = tempF;

        var result = ProductRules.EvaluateHerbicide(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Fact]
    public void Herbicide_TempExactly50_ReturnsPass()
    {
        // 50 < 50 is false → not Marginal → Pass (given wind is fine)
        var w = BaseWeather(); w.TemperatureF = 50;

        var result = ProductRules.EvaluateHerbicide(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    [Fact]
    public void Herbicide_TempExactly85_ReturnsPass()
    {
        // 85 > 85 is false → not Marginal → Pass
        var w = BaseWeather(); w.TemperatureF = 85;

        var result = ProductRules.EvaluateHerbicide(w);

        Assert.Equal(FactorStatus.Pass, result.Status);
    }

    // -----------------------------------------------------------------------
    // Reason is always populated (all rules, representative sample)
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(20,  60, 0,  0)]   // copper cold fail
    [InlineData(65,  91, 0,  0)]   // copper high humidity fail
    [InlineData(65,  60, 0.1, 0)]  // copper recent rain fail
    [InlineData(42,  60, 0,  0)]   // copper marginal temp
    [InlineData(65,  60, 0,  0)]   // copper pass
    public void Copper_Reason_IsNeverEmpty(double tempF, double humidity, double precip1h, double precip24h)
    {
        var w = BaseWeather();
        w.TemperatureF = tempF; w.HumidityPct = humidity;
        w.PrecipPast1hIn = precip1h; w.PrecipPast24hIn = precip24h;

        var result = ProductRules.EvaluateCopper(w);

        Assert.False(string.IsNullOrWhiteSpace(result.Reason));
    }
}
