using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Interfaces;

public interface IAiAdvisoryService
{
    Task<string> GetNarrativeAsync(WeatherSnapshot weather, Crop crop, GrowthStageCode stage, SprayReadinessResult sprayResult, CancellationToken ct = default);
}
