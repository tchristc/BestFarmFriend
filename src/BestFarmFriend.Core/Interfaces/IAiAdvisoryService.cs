using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Core.Interfaces;

public interface IAiAdvisoryService
{
    void Configure(string apiKey, string model = "gpt-4o");

    Task<string> GetNarrativeAsync(
        WeatherSnapshot weather,
        Crop crop,
        GrowthStageCode stage,
        SprayReadinessResult sprayResult,
        CancellationToken ct = default);
}