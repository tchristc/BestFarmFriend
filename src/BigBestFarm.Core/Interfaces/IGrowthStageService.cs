using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Interfaces;

public interface IGrowthStageService
{
    GrowthStageCode EstimateCurrentStage(Crop crop, DateOnly date);
    GrowthStageEntry? GetStageEntry(Crop crop, GrowthStageCode stage);
}
