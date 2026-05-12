namespace BestFarmFriend.Core.Models;

public class WeatherForecastHour
{
    public DateTime Time { get; set; }
    public double TemperatureF { get; set; }
    public double PrecipitationProbabilityPct { get; set; }
    public double PrecipitationIn { get; set; }
    public double WindSpeedMph { get; set; }
    public double WindGustMph { get; set; }
    public WeatherCondition Condition { get; set; }
}
