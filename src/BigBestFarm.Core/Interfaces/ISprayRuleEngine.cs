using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Interfaces;

public interface ISprayRuleEngine
{
    SprayReadinessResult Evaluate(WeatherSnapshot weather);
}
