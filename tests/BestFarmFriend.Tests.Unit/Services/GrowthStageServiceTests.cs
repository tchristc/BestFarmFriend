using BestFarmFriend.Core.Models;
using BestFarmFriend.Core.Services;

namespace BestFarmFriend.Tests.Unit.Services;

public class GrowthStageServiceTests
{
    private static GrowthStageService CreateService() => new();

    private static Crop CropWithStages() => new()
    {
        Name = "Apple",
        GrowthStages = new List<GrowthStageEntry>
        {
            new() { Stage = GrowthStageCode.Dormant,       TypicalStartDayOfYear = 1,   TypicalEndDayOfYear = 60  },
            new() { Stage = GrowthStageCode.SilverTip,     TypicalStartDayOfYear = 61,  TypicalEndDayOfYear = 75  },
            new() { Stage = GrowthStageCode.GreenTip,      TypicalStartDayOfYear = 76,  TypicalEndDayOfYear = 85  },
            new() { Stage = GrowthStageCode.TightCluster,  TypicalStartDayOfYear = 86,  TypicalEndDayOfYear = 95  },
            new() { Stage = GrowthStageCode.PinkPopcorn,   TypicalStartDayOfYear = 96,  TypicalEndDayOfYear = 110 },
            new() { Stage = GrowthStageCode.FullBloom,     TypicalStartDayOfYear = 111, TypicalEndDayOfYear = 125 },
            new() { Stage = GrowthStageCode.PetalFall,     TypicalStartDayOfYear = 126, TypicalEndDayOfYear = 140 },
            new() { Stage = GrowthStageCode.FruitSet,      TypicalStartDayOfYear = 141, TypicalEndDayOfYear = 155 },
            new() { Stage = GrowthStageCode.FirstCover,    TypicalStartDayOfYear = 156, TypicalEndDayOfYear = 200 },
            new() { Stage = GrowthStageCode.PreHarvest,    TypicalStartDayOfYear = 201, TypicalEndDayOfYear = 270 },
            new() { Stage = GrowthStageCode.PostHarvest,   TypicalStartDayOfYear = 271, TypicalEndDayOfYear = 330 },
            new() { Stage = GrowthStageCode.Dormant,       TypicalStartDayOfYear = 331, TypicalEndDayOfYear = 365 }
        }
    };

    [Fact]
    public void EstimateCurrentStage_Jan1_ReturnsDormant()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 1, 1);

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.Dormant, stage);
    }

    [Fact]
    public void EstimateCurrentStage_MidMarch_ReturnsSilverTip()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 3, 5); // ~DOY 65

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.SilverTip, stage);
    }

    [Fact]
    public void EstimateCurrentStage_LateApril_ReturnsPinkPopcorn()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 4, 15); // ~DOY 106

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.PinkPopcorn, stage);
    }

    [Fact]
    public void EstimateCurrentStage_EarlyMay_ReturnsFullBloom()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 5, 1); // ~DOY 122

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.FullBloom, stage);
    }

    [Fact]
    public void EstimateCurrentStage_Summer_ReturnsFirstCover()
    {
        var service = CreateService();
        var crop = CropWithStages();
        var date = new DateOnly(2024, 6, 15); // ~DOY 167

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.FirstCover, stage);
    }

    [Fact]
    public void EstimateCurrentStage_NoCropStages_UsesGenericFallback()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Generic", GrowthStages = new List<GrowthStageEntry>() };

        // DOY 80 => GreenTip in generic fallback
        var date = new DateOnly(2024, 3, 20); // ~DOY 80

        var stage = service.EstimateCurrentStage(crop, date);

        Assert.Equal(GrowthStageCode.GreenTip, stage);
    }

    [Fact]
    public void EstimateCurrentStage_GenericFallback_Jan_ReturnsDormant()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Generic", GrowthStages = new List<GrowthStageEntry>() };

        var stage = service.EstimateCurrentStage(crop, new DateOnly(2024, 1, 15));

        Assert.Equal(GrowthStageCode.Dormant, stage);
    }

    [Fact]
    public void EstimateCurrentStage_GenericFallback_Dec_ReturnsDormant()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Generic", GrowthStages = new List<GrowthStageEntry>() };

        var stage = service.EstimateCurrentStage(crop, new DateOnly(2024, 12, 15));

        Assert.Equal(GrowthStageCode.Dormant, stage);
    }

    [Fact]
    public void EstimateCurrentStage_DateOutsideAllStageRanges_ReturnsFallback()
    {
        // Use crop with only one stage in summer; a winter date has no match
        var service = CreateService();
        var crop = new Crop
        {
            Name = "Test",
            GrowthStages = new List<GrowthStageEntry>
            {
                new() { Stage = GrowthStageCode.FullBloom, TypicalStartDayOfYear = 120, TypicalEndDayOfYear = 135 }
            }
        };

        var stage = service.EstimateCurrentStage(crop, new DateOnly(2024, 1, 10));

        // Falls back to closest — only FullBloom exists
        Assert.Equal(GrowthStageCode.FullBloom, stage);
    }

    [Fact]
    public void GetStageEntry_ExistingStage_ReturnsEntry()
    {
        var service = CreateService();
        var crop = CropWithStages();

        var entry = service.GetStageEntry(crop, GrowthStageCode.FullBloom);

        Assert.NotNull(entry);
        Assert.Equal(GrowthStageCode.FullBloom, entry.Stage);
    }

    [Fact]
    public void GetStageEntry_MissingStage_ReturnsNull()
    {
        var service = CreateService();
        var crop = new Crop { Name = "Empty", GrowthStages = new List<GrowthStageEntry>() };

        var entry = service.GetStageEntry(crop, GrowthStageCode.FullBloom);

        Assert.Null(entry);
    }
}
