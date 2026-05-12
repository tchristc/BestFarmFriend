using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Core.Interfaces;

public interface ISprayRuleEngine
{
    SprayReadinessResult Evaluate(WeatherSnapshot weather);
}
