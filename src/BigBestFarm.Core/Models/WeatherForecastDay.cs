namespace BigBestFarm.Core.Models;

public class WeatherForecastDay
{
    public DateOnly Date { get; set; }
    public double HighF { get; set; }
    public double LowF { get; set; }
    public double PrecipitationSumIn { get; set; }
    public double PrecipProbabilityPct { get; set; }
    public double WindSpeedMaxMph { get; set; }
    public WeatherCondition Condition { get; set; }
    public DateTime SunriseUtc { get; set; }
    public DateTime SunsetUtc { get; set; }
}
