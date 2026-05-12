using BestFarmFriend.Core.Models;

namespace BestFarmFriend.Core.Interfaces;

public interface IGrowthStageService
{
    GrowthStageCode EstimateCurrentStage(Crop crop, DateOnly date);
    GrowthStageEntry? GetStageEntry(Crop crop, GrowthStageCode stage);
}
