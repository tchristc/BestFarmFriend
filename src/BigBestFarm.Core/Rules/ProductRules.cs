using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Rules;

public static class ProductRules
{
    public static ProductReadinessResult EvaluateCopper(WeatherSnapshot w)
    {
        if (w.TemperatureF < 40)
            return Fail(ProductType.Copper, $"Temperature {w.TemperatureF:F1}°F is below 40°F minimum for copper.");
        if (w.HumidityPct > 90)
            return Fail(ProductType.Copper, $"Humidity {w.HumidityPct:F0}% exceeds 90% — copper not recommended.");
        if (w.PrecipPast1hIn > 0)
            return Fail(ProductType.Copper, "Wet foliage (rain in past 1 hour) — copper requires dry foliage.");
        if (w.TemperatureF >= 40 && w.TemperatureF < 45)
            return Marginal(ProductType.Copper, "Temperature is marginal for copper (40–45°F). Efficacy may be reduced.");
        return Pass(ProductType.Copper, "Conditions are suitable for copper-based fungicide application.");
    }

    public static ProductReadinessResult EvaluateSulfur(WeatherSnapshot w)
    {
        if (w.TemperatureF > 90)
            return Fail(ProductType.Sulfur, $"Temperature {w.TemperatureF:F1}°F exceeds 90°F — sulfur phytotoxicity risk.");
        if (w.TemperatureF < 50)
            return Marginal(ProductType.Sulfur, $"Temperature {w.TemperatureF:F1}°F is below 50°F — sulfur efficacy is reduced.");
        return Pass(ProductType.Sulfur, "Conditions are suitable for sulfur-based fungicide application.");
    }

    public static ProductReadinessResult EvaluateOil(WeatherSnapshot w)
    {
        if (w.TemperatureF < 32 || w.TemperatureF > 85)
            return Fail(ProductType.Oil, $"Temperature {w.TemperatureF:F1}°F is outside 32–85°F range required for oil application.");
        if (w.TemperatureF >= 32 && w.TemperatureF < 40)
            return Marginal(ProductType.Oil, "Temperature is at the low end for oil — verify no frost in next 24 hours.");
        return Pass(ProductType.Oil, "Conditions are suitable for horticultural/dormant oil application.");
    }

    public static ProductReadinessResult EvaluateInsecticide(WeatherSnapshot w)
    {
        if (w.WindSpeedMph > 10 || w.WindGustMph > 15)
            return Fail(ProductType.Insecticide, "Wind too high for insecticide — drift risk to pollinators and non-target plants.");
        if (w.TemperatureF < 45 || w.TemperatureF > 95)
            return Marginal(ProductType.Insecticide, "Temperature outside ideal range — check product label.");
        return Pass(ProductType.Insecticide, "Conditions suitable for insecticide. Apply early morning or late evening to protect pollinators.");
    }

    public static ProductReadinessResult EvaluateHerbicide(WeatherSnapshot w)
    {
        if (w.WindSpeedMph > 5)
            return Fail(ProductType.Herbicide, $"Wind {w.WindSpeedMph:F1} mph exceeds 5 mph strict limit for herbicide — drift risk.");
        if (w.TemperatureF < 50 || w.TemperatureF > 85)
            return Marginal(ProductType.Herbicide, $"Temperature {w.TemperatureF:F1}°F is outside ideal 50–85°F range for herbicide activity.");
        return Pass(ProductType.Herbicide, "Conditions are suitable for herbicide application. Use shielded/directed sprayer if wind is 3–5 mph.");
    }

    private static ProductReadinessResult Pass(ProductType type, string reason) =>
        new() { ProductType = type, IsRecommended = true, Status = FactorStatus.Pass, Reason = reason };

    private static ProductReadinessResult Marginal(ProductType type, string reason) =>
        new() { ProductType = type, IsRecommended = false, Status = FactorStatus.Marginal, Reason = reason };

    private static ProductReadinessResult Fail(ProductType type, string reason) =>
        new() { ProductType = type, IsRecommended = false, Status = FactorStatus.Fail, Reason = reason };
}
