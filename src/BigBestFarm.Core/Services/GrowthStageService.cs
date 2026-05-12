using BigBestFarm.Core.Interfaces;
using BigBestFarm.Core.Models;

namespace BigBestFarm.Core.Services;

public class GrowthStageService : IGrowthStageService
{
    public GrowthStageCode EstimateCurrentStage(Crop crop, DateOnly date)
    {
        int doy = date.DayOfYear;

        if (crop.GrowthStages.Count > 0)
        {
            // Find the stage whose typical window contains today
            var match = crop.GrowthStages
                .Where(s => doy >= s.TypicalStartDayOfYear && doy <= s.TypicalEndDayOfYear)
                .OrderBy(s => s.TypicalStartDayOfYear)
                .FirstOrDefault();

            if (match != null)
                return match.Stage;

            // Fall back to closest stage
            var closest = crop.GrowthStages
                .OrderBy(s => Math.Abs(doy - s.TypicalStartDayOfYear))
                .First();
            return closest.Stage;
        }

        // Generic fallback by hemisphere (northern) day of year
        return doy switch
        {
            <= 60 => GrowthStageCode.Dormant,
            <= 75 => GrowthStageCode.SilverTip,
            <= 85 => GrowthStageCode.GreenTip,
            <= 95 => GrowthStageCode.TightCluster,
            <= 110 => GrowthStageCode.PinkPopcorn,
            <= 125 => GrowthStageCode.FullBloom,
            <= 140 => GrowthStageCode.PetalFall,
            <= 155 => GrowthStageCode.FruitSet,
            <= 200 => GrowthStageCode.FirstCover,
            <= 240 => GrowthStageCode.SecondCoverPlus,
            <= 270 => GrowthStageCode.PreHarvest,
            <= 330 => GrowthStageCode.PostHarvest,
            _ => GrowthStageCode.Dormant
        };
    }

    public GrowthStageEntry? GetStageEntry(Crop crop, GrowthStageCode stage) =>
        crop.GrowthStages.FirstOrDefault(s => s.Stage == stage);
}
